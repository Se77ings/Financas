import { useEffect, useState } from "react";
import SubscriptionView from "@/models/Subscription/SubscriptionView";
import CategoryView from "@/models/Category/CategoryView";
import {
    getSubscriptionsByCreditCardAsync,
    getImmediateExpenseSubscriptionsAsync,
    updateSubscriptionAsync,
    deleteSubscriptionAsync,
    addSubscriptionAsync,
    FREQUENCY_LABELS,
    SubscriptionRequest,
} from "@/services/subscriptionServices";
import { getCategoriesAsync } from "@/services/categoryServices";
import { getCreditCardByIdAsync } from "@/services/creditCardServices";
import SubscriptionForm from "@/forms/SubscriptionForm";
import CardExpenseItemForm from "@/forms/CardExpenseItemForm";
import { addCardExpenseItemAsync, CardExpenseItemRequest } from "@/services/cardExpenseServices";
import ImmediateExpenseForm from "@/forms/ImmediateExpenseForm";
import { addImmediateExpenseAsync, ImmediateExpenseRequest } from "@/services/immediateExpenseServices";
import { formatDateBR } from "@/utils/dateFormat";

interface SubscriptionListModalProps {
    /** Sem cartão, o modal gerencia as recorrências à vista. */
    creditCardId?: string;
    /**
     * Fatura (cartão) ou competência (à vista) aberta na tela, quando houver. Com ela, a nova recorrência
     * também é lançada agora; sem ela, só a recorrência é criada e o lançamento surge na próxima competência gerada.
     */
    cardExpenseId?: string;
    effectiveDateId?: string;
    onItemAdded?: () => void;
    onClose: () => void;
}

function toRequest(subscription: SubscriptionView): SubscriptionRequest {
    return {
        description: subscription.description,
        value: subscription.value,
        frequencyType: subscription.frequencyType,
        creditCardId: subscription.creditCardId,
        startDate: subscription.startDate,
        endDate: subscription.endDate,
        isActive: subscription.isActive,
        categoryId: subscription.categoryId,
    };
}

export default function SubscriptionListModal({ creditCardId, cardExpenseId, effectiveDateId, onItemAdded, onClose }: SubscriptionListModalProps) {
    const [subscriptions, setSubscriptions] = useState<SubscriptionView[]>([]);
    const [categories, setCategories] = useState<CategoryView[]>([]);
    const [creditCardName, setCreditCardName] = useState("");
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [editingSubscription, setEditingSubscription] = useState<SubscriptionView | null>(null);
    const [showCreateForm, setShowCreateForm] = useState(false);

    const loadSubscriptions = async () => {
        setError(null);
        try {
            setSubscriptions(await (creditCardId
                ? getSubscriptionsByCreditCardAsync(creditCardId)
                : getImmediateExpenseSubscriptionsAsync()));
        } catch (err) {
            setError(err instanceof Error ? err.message : "Erro ao carregar assinaturas");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadSubscriptions();
        getCategoriesAsync().then(setCategories).catch(err => console.error('Erro ao carregar categorias:', err));
        if (creditCardId)
            getCreditCardByIdAsync(creditCardId).then(card => setCreditCardName(card.name)).catch(err => console.error('Erro ao carregar cartão:', err));
    }, [creditCardId]);

    const categoryName = (categoryId: string | null) =>
        categories.find(c => c.id === categoryId)?.name ?? "—";

    const handleToggleActive = async (subscription: SubscriptionView) => {
        try {
            await updateSubscriptionAsync(subscription.id, { ...toRequest(subscription), isActive: !subscription.isActive });
            await loadSubscriptions();
        } catch (err) {
            alert(err instanceof Error ? err.message : "Erro ao alterar assinatura");
        }
    };

    const handleDelete = async (subscription: SubscriptionView) => {
        if (!confirm(`Remover a assinatura "${subscription.description}"? Os lançamentos já feitos continuam.`)) return;
        try {
            await deleteSubscriptionAsync(subscription.id);
            await loadSubscriptions();
        } catch (err) {
            alert(err instanceof Error ? err.message : "Erro ao remover assinatura");
        }
    };

    const handleSave = async (data: SubscriptionRequest) => {
        if (!editingSubscription) return;
        await updateSubscriptionAsync(editingSubscription.id, data);
        setEditingSubscription(null);
        await loadSubscriptions();
    };

    const handleCreateImmediate = async (data: ImmediateExpenseRequest) => {
        if (effectiveDateId) {
            await addImmediateExpenseAsync(data);
            onItemAdded?.();
        } else {
            await addSubscriptionAsync({
                description: data.description,
                value: data.subTotal,
                frequencyType: data.subscriptionFrequencyType!,
                creditCardId: null,
                startDate: data.subscriptionStartDate!,
                endDate: data.subscriptionEndDate ?? null,
                isActive: true,
                categoryId: data.categoryId,
            });
        }
        await loadSubscriptions();
    };

    const handleCreate = async (data: CardExpenseItemRequest) => {
        if (cardExpenseId) {
            await addCardExpenseItemAsync(cardExpenseId, data);
            onItemAdded?.();
        } else {
            await addSubscriptionAsync({
                description: data.description,
                value: data.subTotal,
                frequencyType: data.subscriptionFrequencyType!,
                creditCardId,
                startDate: data.subscriptionStartDate!,
                endDate: data.subscriptionEndDate ?? null,
                isActive: true,
                categoryId: data.categoryId,
            });
        }
        await loadSubscriptions();
    };

    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 backdrop-blur-sm">
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-[900px] max-w-[95vw] shadow-2xl flex flex-col max-h-[85vh] overflow-hidden">
                <div className="p-4 border-b border-white/5 flex justify-between items-center bg-[#151413] shrink-0">
                    <div>
                        <h3 className="font-bold text-sm uppercase tracking-wider text-blue-400">
                            {creditCardId
                                ? <>🔁 Assinaturas{creditCardName ? ` · ${creditCardName}` : ""}</>
                                : <>🔁 Recorrências à vista</>}
                        </h3>
                        <p className="text-xs text-gray-400 mt-1">
                            {creditCardId
                                ? "Itens recorrentes gerados nas faturas deste cartão a cada nova competência."
                                : "Gastos pagos à vista lançados a cada nova competência, como aluguel, internet e água."}
                        </p>
                    </div>
                    <div className="flex items-center gap-3 shrink-0">
                        <button
                            onClick={() => setShowCreateForm(true)}
                            className="px-3 py-1.5 bg-blue-700 hover:bg-blue-600 transition rounded text-xs font-bold shadow cursor-pointer text-white"
                        >
                            ➕ ADICIONAR RECORRÊNCIA
                        </button>
                        <button
                            onClick={onClose}
                            className="text-gray-500 hover:text-white transition cursor-pointer"
                        >
                            ✕
                        </button>
                    </div>
                </div>

                <div className="flex-grow overflow-y-auto min-h-0">
                    {error && (
                        <div className="m-4 p-3 bg-red-900/20 border border-red-500/30 rounded text-red-400 text-sm">
                            {error}
                        </div>
                    )}

                    {isLoading ? (
                        <div className="text-center py-12 text-gray-500 text-sm">Carregando assinaturas...</div>
                    ) : subscriptions.length === 0 ? (
                        <div className="m-5 text-center py-16 border-2 border-dashed border-white/5 rounded-lg text-gray-500 text-sm italic">
                            {creditCardId ? "Nenhuma assinatura neste cartão." : "Nenhuma recorrência à vista cadastrada."}
                        </div>
                    ) : (
                        <table className="w-full text-left text-sm">
                            <thead className="text-[10px] text-gray-500 uppercase border-b border-white/5 bg-[#151413]">
                                <tr>
                                    <th className="px-4 py-2.5">Descrição</th>
                                    <th className="px-4 py-2.5">Categoria</th>
                                    <th className="px-4 py-2.5">Frequência</th>
                                    <th className="px-4 py-2.5">Vigência</th>
                                    <th className="px-4 py-2.5 text-right">Valor</th>
                                    <th className="px-4 py-2.5 text-center">Status</th>
                                    <th className="px-4 py-2.5 w-20"></th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-white/5 text-xs">
                                {subscriptions.map((subscription) => (
                                    <tr key={subscription.id} className={`hover:bg-white/5 ${subscription.isActive ? '' : 'opacity-50'}`}>
                                        <td className="px-4 py-3 font-semibold text-gray-200">{subscription.description}</td>
                                        <td className="px-4 py-3 text-gray-400">{categoryName(subscription.categoryId)}</td>
                                        <td className="px-4 py-3 text-gray-400">{FREQUENCY_LABELS[subscription.frequencyType]}</td>
                                        <td className="px-4 py-3 text-gray-400">
                                            {formatDateBR(subscription.startDate)} até {subscription.endDate ? formatDateBR(subscription.endDate) : "sem fim"}
                                        </td>
                                        <td className="px-4 py-3 text-right font-mono font-bold text-gray-200">
                                            R$ {subscription.value.toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                                        </td>
                                        <td className="px-4 py-3 text-center">
                                            <button
                                                onClick={() => handleToggleActive(subscription)}
                                                title={subscription.isActive ? "Desativar" : "Ativar"}
                                                className={`px-2 py-0.5 rounded-full text-[9px] uppercase font-bold border cursor-pointer transition ${subscription.isActive
                                                    ? 'bg-green-900/30 text-green-400 border-green-500/20 hover:bg-green-900/50'
                                                    : 'bg-red-900/30 text-red-400 border-red-500/20 hover:bg-red-900/50'}`}
                                            >
                                                {subscription.isActive ? "Ativa" : "Inativa"}
                                            </button>
                                        </td>
                                        <td className="px-4 py-3 text-center">
                                            <div className="flex gap-2 justify-center">
                                                <button
                                                    onClick={() => setEditingSubscription(subscription)}
                                                    className="text-blue-400 hover:text-blue-300 transition text-sm cursor-pointer"
                                                    title="Editar"
                                                >
                                                    ✏️
                                                </button>
                                                <button
                                                    onClick={() => handleDelete(subscription)}
                                                    className="text-red-400 hover:text-red-300 transition text-sm cursor-pointer"
                                                    title="Remover"
                                                >
                                                    🗑️
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    )}
                </div>

                <div className="p-4 border-t border-white/5 bg-[#151413] flex justify-end shrink-0">
                    <button
                        onClick={onClose}
                        className="px-4 py-1.5 rounded border border-white/10 text-white hover:bg-white/5 transition text-xs font-semibold cursor-pointer"
                    >
                        Fechar
                    </button>
                </div>

                {showCreateForm && !creditCardId && (
                    <ImmediateExpenseForm
                        effectiveDateId={effectiveDateId}
                        lockRecurring
                        onSubmit={handleCreateImmediate}
                        onClose={() => setShowCreateForm(false)}
                    />
                )}

                {showCreateForm && creditCardId && (
                    <CardExpenseItemForm
                        cardExpenseId={cardExpenseId}
                        creditCardId={creditCardId}
                        lockRecurring
                        onSubmit={handleCreate}
                        onClose={() => setShowCreateForm(false)}
                    />
                )}

                {editingSubscription && (
                    <SubscriptionForm
                        subscription={editingSubscription}
                        onSubmit={handleSave}
                        onClose={() => setEditingSubscription(null)}
                    />
                )}
            </div>
        </div>
    );
}
