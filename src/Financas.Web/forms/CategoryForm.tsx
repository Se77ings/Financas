import { useState } from 'react';
import { CategoryRequest } from '@/services/categoryServices';

interface CategoryFormProps {
    initialData?: {
        id: string;
        name: string;
        description: string | null;
    } | null;
    initialName?: string;
    onSubmit: (data: CategoryRequest) => Promise<void>;
    onClose: () => void;
    isLoading?: boolean;
}

export default function CategoryForm({
    initialData,
    initialName = '',
    onSubmit,
    onClose,
    isLoading = false,
}: CategoryFormProps) {
    const [formData, setFormData] = useState({
        name: initialData?.name || initialName,
        description: initialData?.description || '',
    });
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        // Aberto de dentro de outro formulário (via portal), o submit sintético subiria até ele.
        e.stopPropagation();
        setError(null);
        setIsSubmitting(true);

        try {
            await onSubmit(formData);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Erro ao salvar categoria');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-96 shadow-2xl">
                <div className="p-4 border-b border-white/5 flex justify-between items-center">
                    <h3 className="font-semibold text-sm uppercase tracking-wider">
                        {initialData ? 'Editar Categoria' : 'Nova Categoria'}
                    </h3>
                    <button
                        onClick={onClose}
                        className="text-gray-500 hover:text-white transition cursor-pointer"
                    >
                        ✕
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="p-4 space-y-4">
                    {error && (
                        <div className="p-3 bg-red-900/20 border border-red-500/30 rounded text-red-400 text-sm">
                            {error}
                        </div>
                    )}

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Nome
                        </label>
                        <input
                            tabIndex={1}
                            autoFocus
                            type="text"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            required
                            maxLength={100}
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                            placeholder="Ex: Mercado"
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Descrição <span className="font-normal text-gray-600">(opcional)</span>
                        </label>
                        <input
                            tabIndex={2}
                            type="text"
                            name="description"
                            value={formData.description}
                            onChange={handleChange}
                            maxLength={100}
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                            placeholder="Ex: Compras de supermercado e feira"
                        />
                    </div>

                    <div className="flex gap-2 pt-4">
                        <button
                            tabIndex={4}
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 rounded border border-white/10 text-white hover:bg-white/5 transition text-sm font-medium cursor-pointer"
                        >
                            Cancelar
                        </button>
                        <button
                            tabIndex={3}
                            type="submit"
                            disabled={isSubmitting || isLoading}
                            className="flex-1 px-4 py-2 rounded bg-green-700 hover:bg-green-600 disabled:bg-green-900/50 disabled:cursor-not-allowed text-white transition text-sm font-medium cursor-pointer"
                        >
                            {isSubmitting ? 'Salvando...' : 'Salvar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
