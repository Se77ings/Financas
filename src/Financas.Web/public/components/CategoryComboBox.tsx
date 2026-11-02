import { useEffect, useId, useRef, useState } from "react";
import { createPortal } from "react-dom";
import CategoryView from "@/models/Category/CategoryView";
import { addCategoryAsync, getCategoriesAsync, CategoryRequest } from "@/services/categoryServices";
import CategoryForm from "@/forms/CategoryForm";

const VISIBLE_LIMIT = 10;

type ComboOption = { kind: 'category'; category: CategoryView } | { kind: 'add' };

interface CategoryComboBoxProps {
    value: string | null;
    onChange: (categoryId: string | null) => void;
    tabIndex?: number;
    excludeIds?: string[];
}

function normalize(text: string): string {
    return text.normalize('NFD').replace(/\p{Diacritic}/gu, '').toLowerCase().trim();
}

function sortByName(categories: CategoryView[]): CategoryView[] {
    return [...categories].sort((a, b) => a.name.localeCompare(b.name, 'pt-BR'));
}

export function CategoryComboBox({ value, onChange, tabIndex, excludeIds = [] }: CategoryComboBoxProps) {
    const [categories, setCategories] = useState<CategoryView[]>([]);
    const [query, setQuery] = useState('');
    const [isSearching, setIsSearching] = useState(false);
    const [isOpen, setIsOpen] = useState(false);
    const [highlighted, setHighlighted] = useState(0);
    const [position, setPosition] = useState<{ top: number; left: number; width: number } | null>(null);
    const [showCreateForm, setShowCreateForm] = useState(false);
    const [pendingName, setPendingName] = useState('');

    const inputRef = useRef<HTMLInputElement>(null);
    // Ao devolver o foco depois de criar uma categoria, a lista não deve reabrir.
    const skipNextOpenRef = useRef(false);
    const listId = useId();

    useEffect(() => {
        getCategoriesAsync()
            .then(data => setCategories(sortByName(data)))
            .catch(err => console.error('Erro ao carregar categorias:', err));
    }, []);

    const selected = categories.find(c => c.id === value) ?? null;
    const term = isSearching ? normalize(query) : '';
    const available = categories.filter(c => !excludeIds.includes(c.id));
    const matches = term ? available.filter(c => normalize(c.name).includes(term)) : available;
    const options: ComboOption[] = matches
        .slice(0, VISIBLE_LIMIT)
        .map(category => ({ kind: 'category' as const, category }));
    options.push({ kind: 'add' });

    const updatePosition = () => {
        const rect = inputRef.current?.getBoundingClientRect();
        if (rect) setPosition({ top: rect.bottom + 4, left: rect.left, width: rect.width });
    };

    // Os modais dos formulários rolam (overflow-y-auto): a lista fica num portal com posição fixa
    // e acompanha o input quando qualquer ancestral rola.
    useEffect(() => {
        if (!isOpen) return;
        window.addEventListener('scroll', updatePosition, true);
        window.addEventListener('resize', updatePosition);
        return () => {
            window.removeEventListener('scroll', updatePosition, true);
            window.removeEventListener('resize', updatePosition);
        };
    }, [isOpen]);

    const open = () => {
        updatePosition();
        setIsOpen(true);
    };

    const close = () => {
        setIsOpen(false);
        setIsSearching(false);
        setQuery('');
    };

    const choose = (option: ComboOption) => {
        if (option.kind === 'category') {
            onChange(option.category.id);
            close();
            return;
        }

        setPendingName(query.trim());
        close();
        setShowCreateForm(true);
    };

    const handleFocus = () => {
        if (skipNextOpenRef.current) {
            skipNextOpenRef.current = false;
            return;
        }
        const selectedIndex = selected ? available.slice(0, VISIBLE_LIMIT).indexOf(selected) : -1;
        setHighlighted(Math.max(0, selectedIndex));
        open();
    };

    const handleBlur = () => {
        if (isSearching && query.trim() === '') onChange(null);
        close();
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setQuery(e.target.value);
        setIsSearching(true);
        setHighlighted(0);
        if (!isOpen) open();
    };

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                if (!isOpen) open();
                setHighlighted(prev => Math.min(prev + 1, options.length - 1));
                break;
            case 'ArrowUp':
                e.preventDefault();
                setHighlighted(prev => Math.max(prev - 1, 0));
                break;
            case 'Enter':
                // Com a lista fechada, o Enter segue para o formulário e salva o lançamento.
                if (!isOpen) return;
                e.preventDefault();
                if (options[highlighted]) choose(options[highlighted]);
                break;
            case 'Escape':
                if (!isOpen) return;
                e.preventDefault();
                e.stopPropagation();
                close();
                break;
            case 'Tab': {
                const option = options[highlighted];
                if (isOpen && isSearching && option?.kind === 'category') choose(option);
                break;
            }
        }
    };

    const handleCreate = async (data: CategoryRequest) => {
        const created = await addCategoryAsync(data);
        setCategories(prev => sortByName([...prev, created]));
        onChange(created.id);
        setShowCreateForm(false);
        skipNextOpenRef.current = true;
        inputRef.current?.focus();
    };

    return (
        <>
            <input
                ref={inputRef}
                type="text"
                role="combobox"
                aria-expanded={isOpen}
                aria-controls={listId}
                aria-autocomplete="list"
                autoComplete="off"
                tabIndex={tabIndex}
                value={isSearching ? query : selected?.name ?? ''}
                onChange={handleInputChange}
                onFocus={handleFocus}
                onBlur={handleBlur}
                onKeyDown={handleKeyDown}
                placeholder="Sem categoria"
                className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
            />

            {isOpen && position && createPortal(
                <ul
                    id={listId}
                    role="listbox"
                    style={{ top: position.top, left: position.left, width: position.width }}
                    className="fixed z-[60] max-h-72 overflow-y-auto bg-[#1a1917] border border-white/10 rounded shadow-2xl py-1 text-sm"
                >
                    {options.length === 1 && (
                        <li className="px-3 py-2 text-gray-500 italic">
                            {term ? 'Nenhuma categoria encontrada.' : 'Nenhuma categoria cadastrada.'}
                        </li>
                    )}
                    {options.map((option, index) => (
                        <li
                            key={option.kind === 'category' ? option.category.id : 'add'}
                            role="option"
                            aria-selected={index === highlighted}
                            // mousedown tira o foco do input antes do click; sem isso o blur fecharia a lista.
                            onMouseDown={(e) => e.preventDefault()}
                            onClick={() => choose(option)}
                            onMouseEnter={() => setHighlighted(index)}
                            className={`px-3 py-2 cursor-pointer ${index === highlighted ? 'bg-blue-600 text-white' : 'text-gray-200'} ${option.kind === 'add' ? 'border-t border-white/10 text-blue-300 font-semibold' : ''}`}
                        >
                            {option.kind === 'category'
                                ? option.category.name
                                : '➕ Adicionar categoria...'}
                        </li>
                    ))}
                </ul>,
                document.body
            )}

            {showCreateForm && createPortal(
                <CategoryForm
                    initialName={pendingName}
                    onSubmit={handleCreate}
                    onClose={() => setShowCreateForm(false)}
                />,
                document.body
            )}
        </>
    );
}
