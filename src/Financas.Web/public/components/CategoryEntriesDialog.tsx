import { useState } from "react";
import CategoryView from "@/models/Category/CategoryView";
import { deleteCategoryAsync } from "@/services/categoryServices";
import CategoryEntriesPanel from "@/public/components/CategoryEntriesPanel";

interface CategoryEntriesDialogProps {
    category: CategoryView;
    onClose: () => void;
    onChanged: () => void;
    onDeleted: () => void;
}

export default function CategoryEntriesDialog({ category, onClose, onChanged, onDeleted }: CategoryEntriesDialogProps) {
    const [remaining, setRemaining] = useState<number | null>(null);
    const [isDeleting, setIsDeleting] = useState(false);

    const handleDelete = async () => {
        if (!confirm(`Tem certeza que deseja excluir a categoria "${category.name}"?`)) return;

        setIsDeleting(true);
        try {
            await deleteCategoryAsync(category.id);
            onDeleted();
        } catch (err) {
            alert(err instanceof Error ? err.message : "Erro ao excluir categoria.");
        } finally {
            setIsDeleting(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 backdrop-blur-sm">
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-[1100px] max-w-[95vw] h-[85vh] shadow-2xl flex flex-col overflow-hidden">
                <div className="p-4 border-b border-white/5 bg-[#151413] flex justify-between items-center shrink-0">
                    <div>
                        <h3 className="font-bold text-sm uppercase tracking-wider text-blue-400">
                            Lançamentos de {category.name}
                        </h3>
                        <p className="text-xs text-gray-400 mt-1">
                            Selecione os lançamentos que deseja mover para outra categoria. Para excluir a categoria, ela precisa estar vazia.
                        </p>
                    </div>
                    <button
                        onClick={onClose}
                        className="text-gray-500 hover:text-white transition cursor-pointer"
                    >
                        ✕
                    </button>
                </div>

                <CategoryEntriesPanel categoryId={category.id} onChanged={onChanged} onLoaded={setRemaining} />

                <div className="p-4 border-t border-white/5 bg-[#151413] flex justify-between items-center shrink-0">
                    <button
                        onClick={handleDelete}
                        disabled={remaining !== 0 || isDeleting}
                        title={remaining === 0 ? undefined : "Mova todos os lançamentos antes de excluir"}
                        className="px-4 py-1.5 rounded bg-red-800 hover:bg-red-700 disabled:bg-red-950/40 disabled:text-gray-500 disabled:cursor-not-allowed text-white transition text-xs font-semibold cursor-pointer"
                    >
                        {isDeleting ? "Excluindo..." : "🗑️ Excluir categoria"}
                    </button>
                    <button
                        onClick={onClose}
                        className="px-4 py-1.5 rounded border border-white/10 text-white hover:bg-white/5 transition text-xs font-semibold cursor-pointer"
                    >
                        Fechar
                    </button>
                </div>
            </div>
        </div>
    );
}
