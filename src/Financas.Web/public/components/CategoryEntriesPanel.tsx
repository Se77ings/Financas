import { Fragment, useEffect, useState } from "react";
import EntriesByDescriptionView from "@/models/Entry/EntriesByDescriptionView";
import EntryView, { EntryType } from "@/models/Entry/EntryView";
import { getEntriesByCategoryAsync, getUncategorizedEntriesAsync, linkPayablesToCategoryAsync, linkReceivablesToCategoryAsync, renameEntriesAsync } from "@/services/categoryServices";
import { CategoryComboBox } from "@/public/components/CategoryComboBox";
import { formatDateBR } from "@/utils/dateFormat";
import { formatEffectiveDateLabel } from "@/utils/calendarMonth";

const formatCurrency = (value: number) =>
    value.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });

const TYPE_LABELS: Record<EntryType, string> = {
    ImmediateExpense: "Gasto",
    CardExpenseItem: "Cartão",
    Receivable: "Recebimento",
};

// Recebimentos e contas a pagar são vinculados por endpoints diferentes.
function splitByType(entries: EntryView[]) {
    return {
        payableIds: entries.filter(e => e.type !== 'Receivable').map(e => e.id),
        receivableIds: entries.filter(e => e.type === 'Receivable').map(e => e.id),
    };
}

function GroupCheckbox({ checked, indeterminate, onChange }: { checked: boolean; indeterminate: boolean; onChange: () => void }) {
    return (
        <input
            type="checkbox"
            checked={checked}
            ref={(el) => { if (el) el.indeterminate = indeterminate; }}
            onChange={onChange}
            className="h-4 w-4 accent-blue-500 cursor-pointer"
        />
    );
}

interface CategoryEntriesPanelProps {
    categoryId: string | null;
    onChanged?: () => void;
    onLoaded?: (entriesCount: number) => void;
}

export default function CategoryEntriesPanel({ categoryId, onChanged, onLoaded }: CategoryEntriesPanelProps) {
    const [groups, setGroups] = useState<EntriesByDescriptionView[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [errorMsg, setErrorMsg] = useState("");

    const [expanded, setExpanded] = useState<Set<string>>(new Set());
    const [selectedIds, setSelectedIds] = useState<Set<string>>(new Set());

    const [linkCategoryId, setLinkCategoryId] = useState<string | null>(null);
    const [isLinking, setIsLinking] = useState(false);

    const [editingGroup, setEditingGroup] = useState<string | null>(null);
    const [editValue, setEditValue] = useState("");
    const [isRenaming, setIsRenaming] = useState(false);

    const loadGroups = async () => {
        setIsLoading(true);
        setErrorMsg("");
        try {
            const data = await (categoryId ? getEntriesByCategoryAsync(categoryId) : getUncategorizedEntriesAsync());
            setGroups(data);
            onLoaded?.(data.reduce((sum, g) => sum + g.count, 0));
            // Mantém só a seleção de lançamentos que continuam nesta lista.
            const remaining = new Set(data.flatMap(g => g.entries.map(e => e.id)));
            setSelectedIds(prev => new Set([...prev].filter(id => remaining.has(id))));
        } catch (err) {
            setErrorMsg(err instanceof Error ? err.message : "Erro ao carregar lançamentos.");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadGroups();
    }, []);

    const allEntries = groups.flatMap(g => g.entries);
    const selectedEntries = allEntries.filter(e => selectedIds.has(e.id));
    const allSelected = allEntries.length > 0 && selectedEntries.length === allEntries.length;

    const toggleExpanded = (key: string) => {
        setExpanded(prev => {
            const next = new Set(prev);
            if (next.has(key)) next.delete(key); else next.add(key);
            return next;
        });
    };

    const setSelection = (ids: string[], selected: boolean) => {
        setSelectedIds(prev => {
            const next = new Set(prev);
            ids.forEach(id => selected ? next.add(id) : next.delete(id));
            return next;
        });
    };

    const toggleGroup = (group: EntriesByDescriptionView) => {
        const ids = group.entries.map(e => e.id);
        const isFullySelected = ids.every(id => selectedIds.has(id));
        setSelection(ids, !isFullySelected);
    };

    const handleLink = async () => {
        if (!linkCategoryId || selectedEntries.length === 0) return;

        setIsLinking(true);
        try {
            const { payableIds, receivableIds } = splitByType(selectedEntries);
            if (payableIds.length > 0)
                await linkPayablesToCategoryAsync({ ids: payableIds, categoryId: linkCategoryId });
            if (receivableIds.length > 0)
                await linkReceivablesToCategoryAsync({ ids: receivableIds, categoryId: linkCategoryId });

            setSelectedIds(new Set());
            setLinkCategoryId(null);
            await loadGroups();
            onChanged?.();
        } catch (err) {
            alert(err instanceof Error ? err.message : "Erro ao vincular categoria.");
        } finally {
            setIsLinking(false);
        }
    };

    const startRename = (group: EntriesByDescriptionView) => {
        setEditingGroup(group.description);
        setEditValue(group.description);
    };

    const cancelRename = () => {
        setEditingGroup(null);
        setEditValue("");
    };

    const handleRename = async (group: EntriesByDescriptionView) => {
        const description = editValue.trim();
        if (!description || description === group.description) {
            cancelRename();
            return;
        }

        setIsRenaming(true);
        try {
            await renameEntriesAsync({ ...splitByType(group.entries), description });
            cancelRename();
            await loadGroups();
            onChanged?.();
        } catch (err) {
            alert(err instanceof Error ? err.message : "Erro ao atualizar descrição.");
        } finally {
            setIsRenaming(false);
        }
    };

    return (
        <div className="flex flex-col flex-1 min-h-0 w-full">
            {selectedEntries.length > 0 && (
                <div className="px-4 py-3 border-b border-white/10 bg-[#1a1917] shrink-0 flex flex-wrap items-center gap-3">
                    <span className="text-sm font-semibold text-blue-300">
                        {selectedEntries.length} selecionado(s)
                    </span>
                    <div className="w-64">
                        <CategoryComboBox
                            value={linkCategoryId}
                            onChange={setLinkCategoryId}
                            excludeIds={categoryId ? [categoryId] : []}
                        />
                    </div>
                    <button
                        onClick={handleLink}
                        disabled={!linkCategoryId || isLinking}
                        className="px-3.5 py-2 bg-blue-600 hover:bg-blue-500 disabled:bg-blue-900/50 disabled:cursor-not-allowed text-xs font-semibold rounded transition cursor-pointer"
                    >
                        {isLinking
                            ? (categoryId ? "Movendo..." : "Vinculando...")
                            : (categoryId ? "Mover para a Categoria" : "Vincular à Categoria")}
                    </button>
                    <button
                        onClick={() => setSelectedIds(new Set())}
                        className="px-3 py-2 text-xs text-gray-400 hover:text-white transition cursor-pointer"
                    >
                        Limpar seleção
                    </button>
                </div>
            )}

            <div className="flex-1 overflow-auto">
                {errorMsg && (
                    <div className="m-4 p-3.5 bg-red-950/60 border border-red-500/20 text-red-200 text-sm rounded-lg">
                        {errorMsg}
                    </div>
                )}

                {isLoading ? (
                    <div className="flex items-center justify-center py-20">
                        <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-500" />
                    </div>
                ) : groups.length === 0 ? (
                    <div className="m-6 flex flex-col items-center justify-center py-20 gap-3 rounded-2xl border-2 border-dashed border-white/10 bg-white/5">
                        <p className="text-gray-400 text-sm">
                            {categoryId ? "Nenhum lançamento nesta categoria." : "Todos os lançamentos já têm categoria."}
                        </p>
                    </div>
                ) : (
                    <div className="bg-black/20">
                        <table className="w-full text-left border-collapse">
                            <thead>
                                <tr className="bg-white/5 text-xs font-semibold text-gray-400 uppercase tracking-wider border-b border-white/10">
                                    <th className="py-3 px-4 w-10">
                                        <GroupCheckbox
                                            checked={allSelected}
                                            indeterminate={!allSelected && selectedEntries.length > 0}
                                            onChange={() => setSelection(allEntries.map(e => e.id), !allSelected)}
                                        />
                                    </th>
                                    <th className="py-3 px-2 w-8"></th>
                                    <th className="py-3 px-4">Lançamento</th>
                                    <th className="py-3 px-4 text-center w-32">Ocorrências</th>
                                    <th className="py-3 px-4 w-16"></th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-white/5 text-sm">
                                {groups.map((group) => {
                                    const key = group.description;
                                    const isExpanded = expanded.has(key);
                                    const selectedInGroup = group.entries.filter(e => selectedIds.has(e.id)).length;
                                    const isEditing = editingGroup === key;

                                    return (
                                        <Fragment key={key}>
                                            <tr className="hover:bg-white/5 transition">
                                                <td className="py-3 px-4">
                                                    <GroupCheckbox
                                                        checked={selectedInGroup === group.entries.length}
                                                        indeterminate={selectedInGroup > 0 && selectedInGroup < group.entries.length}
                                                        onChange={() => toggleGroup(group)}
                                                    />
                                                </td>
                                                <td className="py-3 px-2">
                                                    <button
                                                        onClick={() => toggleExpanded(key)}
                                                        className={`text-gray-400 hover:text-white transition cursor-pointer ${isExpanded ? 'rotate-90' : ''}`}
                                                        title={isExpanded ? "Recolher" : "Expandir"}
                                                    >
                                                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><polyline points="9 18 15 12 9 6"></polyline></svg>
                                                    </button>
                                                </td>
                                                <td className="py-3 px-4">
                                                    {isEditing ? (
                                                        <input
                                                            autoFocus
                                                            value={editValue}
                                                            maxLength={100}
                                                            disabled={isRenaming}
                                                            onChange={(e) => setEditValue(e.target.value)}
                                                            onKeyDown={(e) => {
                                                                if (e.key === 'Enter') handleRename(group);
                                                                if (e.key === 'Escape') cancelRename();
                                                            }}
                                                            onBlur={cancelRename}
                                                            className="w-full px-2 py-1 bg-[#0f0e0c] border border-blue-500 rounded text-white focus:outline-none"
                                                        />
                                                    ) : (
                                                        <span className="font-medium">{group.description}</span>
                                                    )}
                                                </td>
                                                <td className="py-3 px-4 text-center text-gray-300 font-mono">{group.count}</td>
                                                <td className="py-3 px-4 text-right">
                                                    {!isEditing && (
                                                        <button
                                                            onClick={() => startRename(group)}
                                                            className="p-1 rounded text-gray-400 hover:text-blue-400 hover:bg-white/5 transition cursor-pointer"
                                                            title="Corrigir descrição de todos os lançamentos do grupo"
                                                        >
                                                            ✏️
                                                        </button>
                                                    )}
                                                </td>
                                            </tr>

                                            {isExpanded && group.entries.map((entry) => (
                                                <tr key={entry.id} className="bg-black/30 text-xs text-gray-300">
                                                    <td className="py-2 px-4"></td>
                                                    <td className="py-2 px-2">
                                                        <input
                                                            type="checkbox"
                                                            checked={selectedIds.has(entry.id)}
                                                            onChange={(e) => setSelection([entry.id], e.target.checked)}
                                                            className="h-3.5 w-3.5 accent-blue-500 cursor-pointer"
                                                        />
                                                    </td>
                                                    <td className="py-2 px-4" colSpan={3}>
                                                        <div className="grid grid-cols-5 gap-3 items-center">
                                                            <span className={entry.type === 'Receivable' ? 'text-green-400' : 'text-gray-400'}>
                                                                {TYPE_LABELS[entry.type]}
                                                                {entry.creditCardName ? ` · ${entry.creditCardName}` : ''}
                                                            </span>
                                                            <span>{formatDateBR(entry.date)}</span>
                                                            <span className="uppercase">{formatEffectiveDateLabel(entry.month, entry.year)}</span>
                                                            <span className="text-gray-500">
                                                                {entry.quantityInstallments && entry.quantityInstallments > 1
                                                                    ? `Parcela ${entry.currentInstallment}/${entry.quantityInstallments}`
                                                                    : ''}
                                                            </span>
                                                            <span className="text-right font-mono font-semibold">{formatCurrency(entry.total)}</span>
                                                        </div>
                                                    </td>
                                                </tr>
                                            ))}
                                        </Fragment>
                                    );
                                })}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
}
