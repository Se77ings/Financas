"use client"

import { useEffect, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import CategoryView from "@/models/Category/CategoryView";
import { getCategoriesAsync, addCategoryAsync, updateCategoryAsync, deleteCategoryAsync, CategoryRequest } from "@/services/categoryServices";
import CategoryForm from "@/forms/CategoryForm";
import CategoryEntriesDialog from "@/public/components/CategoryEntriesDialog";
import CategoryEntriesPanel from "@/public/components/CategoryEntriesPanel";

type Tab = 'categories' | 'uncategorized';

export default function CategoriesPage() {
    const router = useRouter();
    const searchParams = useSearchParams();
    // Preenchido pelo "Resolver" do dashboard: a aba "Sem categoria" abre filtrada pela competência de lá.
    const resolverEffectiveDateId = searchParams?.get("effectiveDateId") ?? null;
    const [tab, setTab] = useState<Tab>(searchParams?.get("tab") === 'uncategorized' ? 'uncategorized' : 'categories');

    const selectTab = (value: Tab) => {
        setTab(value);
        // Ao trocar de aba manualmente, o filtro deixa de vir do dashboard.
        if (resolverEffectiveDateId) router.replace("/categories");
    };

    const [categories, setCategories] = useState<CategoryView[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [errorMsg, setErrorMsg] = useState("");

    const [showForm, setShowForm] = useState(false);
    const [editingCategory, setEditingCategory] = useState<CategoryView | null>(null);
    const [viewingCategory, setViewingCategory] = useState<CategoryView | null>(null);

    const loadCategories = async () => {
        setErrorMsg("");
        try {
            const data = await getCategoriesAsync();
            setCategories([...data].sort((a, b) => a.name.localeCompare(b.name, 'pt-BR')));
        } catch (err) {
            setErrorMsg(err instanceof Error ? err.message : "Erro ao carregar categorias.");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadCategories();
    }, []);

    const handleSave = async (data: CategoryRequest) => {
        if (editingCategory) {
            await updateCategoryAsync(editingCategory.id, data);
        } else {
            await addCategoryAsync(data);
        }
        setShowForm(false);
        setEditingCategory(null);
        await loadCategories();
    };

    const handleDelete = async (category: CategoryView) => {
        if (!confirm(`Tem certeza que deseja remover a categoria "${category.name}"?`)) return;
        try {
            await deleteCategoryAsync(category.id);
            await loadCategories();
        } catch (err) {
            alert(err instanceof Error ? err.message : "Erro ao remover categoria.");
        }
    };

    const tabClass = (value: Tab) =>
        `px-4 py-2.5 text-xs font-semibold uppercase tracking-wider border-b-2 transition cursor-pointer ${tab === value
            ? 'border-blue-500 text-white'
            : 'border-transparent text-gray-400 hover:text-gray-200'}`;

    return (
        <div className="flex flex-col flex-1 min-h-0 w-full h-full text-gray-100 bg-blue-950">
            <header className="px-5 py-4 border-b border-white/10 shrink-0 flex items-center justify-between bg-blue-900/30">
                <div className="flex items-center gap-3">
                    <button
                        onClick={() => router.push("/")}
                        className="p-1.5 rounded-lg text-gray-400 hover:text-white hover:bg-white/10 transition cursor-pointer"
                        title="Voltar ao início"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth="2">
                            <path strokeLinecap="round" strokeLinejoin="round" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                    </button>
                    <h1 className="text-base font-bold tracking-tight">Gerenciar Categorias</h1>
                </div>
                {tab === 'categories' && (
                    <button
                        onClick={() => {
                            setEditingCategory(null);
                            setShowForm(true);
                        }}
                        className="px-3.5 py-1.5 bg-blue-600 hover:bg-blue-500 active:bg-blue-700 text-xs font-semibold rounded-lg shadow transition cursor-pointer"
                    >
                        ➕ Nova Categoria
                    </button>
                )}
            </header>

            <nav className="px-5 border-b border-white/10 shrink-0 flex gap-1">
                <button className={tabClass('categories')} onClick={() => { selectTab('categories'); loadCategories(); }}>
                    Categorias
                </button>
                <button className={tabClass('uncategorized')} onClick={() => selectTab('uncategorized')}>
                    Sem categoria
                </button>
            </nav>

            {tab === 'uncategorized' ? (
                <CategoryEntriesPanel categoryId={null} initialEffectiveDateId={resolverEffectiveDateId} onChanged={loadCategories} />
            ) : (
                <main className="flex-1 overflow-auto p-6 max-w-5xl w-full mx-auto">
                    {errorMsg && (
                        <div className="mb-4 p-3.5 bg-red-950/60 border border-red-500/20 text-red-200 text-sm rounded-lg">
                            {errorMsg}
                        </div>
                    )}

                    {isLoading ? (
                        <div className="flex items-center justify-center py-20">
                            <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-blue-500" />
                        </div>
                    ) : categories.length === 0 ? (
                        <div className="flex flex-col items-center justify-center py-20 gap-3 rounded-2xl border-2 border-dashed border-white/10 bg-white/5">
                            <p className="text-gray-400 text-sm">Nenhuma categoria cadastrada.</p>
                        </div>
                    ) : (
                        <div className="overflow-hidden rounded-xl border border-white/15 bg-black/20 shadow-2xl">
                            <table className="w-full text-left border-collapse">
                                <thead>
                                    <tr className="bg-white/5 text-xs font-semibold text-gray-400 uppercase tracking-wider border-b border-white/10">
                                        <th className="py-3 px-4">Nome</th>
                                        <th className="py-3 px-4">Descrição</th>
                                        <th className="py-3 px-4 text-center w-32">Lançamentos</th>
                                        <th className="py-3 px-4 text-right w-32">Ações</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-white/5 text-sm">
                                    {categories.map((category) => (
                                        <tr key={category.id} className="hover:bg-white/5 transition">
                                            <td className="py-3 px-4 font-medium">{category.name}</td>
                                            <td className="py-3 px-4 text-gray-400">{category.description ?? '—'}</td>
                                            <td className="py-3 px-4 text-center font-mono text-gray-300">{category.entriesCount}</td>
                                            <td className="py-3 px-4">
                                                <div className="flex justify-end gap-1.5">
                                                    <button
                                                        onClick={() => {
                                                            setEditingCategory(category);
                                                            setShowForm(true);
                                                        }}
                                                        className="p-1 rounded hover:bg-white/5 transition cursor-pointer"
                                                        title="Editar"
                                                    >
                                                        ✏️
                                                    </button>
                                                    <button
                                                        onClick={() => setViewingCategory(category)}
                                                        className="p-1 rounded hover:bg-white/5 transition cursor-pointer"
                                                        title="Ver e mover lançamentos"
                                                    >
                                                        🔀
                                                    </button>
                                                    <button
                                                        onClick={() => handleDelete(category)}
                                                        disabled={category.entriesCount > 0}
                                                        className="p-1 rounded hover:bg-white/5 transition cursor-pointer disabled:opacity-30 disabled:cursor-not-allowed"
                                                        title={category.entriesCount > 0 ? "Migre os lançamentos antes de excluir" : "Remover"}
                                                    >
                                                        🗑️
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    )}
                </main>
            )}

            {showForm && (
                <CategoryForm
                    initialData={editingCategory}
                    onSubmit={handleSave}
                    onClose={() => {
                        setShowForm(false);
                        setEditingCategory(null);
                    }}
                />
            )}

            {viewingCategory && (
                <CategoryEntriesDialog
                    category={viewingCategory}
                    onClose={() => setViewingCategory(null)}
                    onChanged={loadCategories}
                    onDeleted={() => {
                        setViewingCategory(null);
                        loadCategories();
                    }}
                />
            )}
        </div>
    );
}
