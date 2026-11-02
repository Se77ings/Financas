import { useState } from 'react';
import SubscriptionView from '@/models/Subscription/SubscriptionView';
import { FrequencyType, FREQUENCY_LABELS, SubscriptionRequest } from '@/services/subscriptionServices';
import { ValueComponent } from '@/public/components/ValueComponent';
import { CategoryComboBox } from '@/public/components/CategoryComboBox';
import { toInputDate } from '@/utils/dateFormat';

interface SubscriptionFormProps {
    subscription: SubscriptionView;
    onSubmit: (data: SubscriptionRequest) => Promise<void>;
    onClose: () => void;
}

export default function SubscriptionForm({ subscription, onSubmit, onClose }: SubscriptionFormProps) {
    const [formData, setFormData] = useState({
        description: subscription.description,
        value: subscription.value,
        frequencyType: subscription.frequencyType,
        startDate: toInputDate(subscription.startDate),
        endDate: toInputDate(subscription.endDate),
        isActive: subscription.isActive,
        categoryId: subscription.categoryId,
    });
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (formData.value <= 0) {
            setError('O valor deve ser maior que zero.');
            return;
        }

        if (!formData.categoryId) {
            setError('Selecione uma categoria.');
            return;
        }

        if (formData.endDate && formData.endDate < formData.startDate) {
            setError('A data de fim não pode ser anterior à data de início.');
            return;
        }

        setIsSubmitting(true);
        try {
            await onSubmit({
                description: formData.description,
                value: formData.value,
                frequencyType: formData.frequencyType,
                creditCardId: subscription.creditCardId,
                startDate: formData.startDate,
                endDate: formData.endDate || null,
                isActive: formData.isActive,
                categoryId: formData.categoryId,
            });
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Erro ao salvar assinatura');
        } finally {
            setIsSubmitting(false);
        }
    };

    const inputClass = "w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition";

    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-96 shadow-2xl max-h-screen overflow-y-auto">
                <div className="p-4 border-b border-white/5 flex justify-between items-center">
                    <h3 className="font-semibold text-sm uppercase tracking-wider">Editar Assinatura</h3>
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

                    <p className="text-xs text-gray-400">
                        As alterações valem para as próximas competências geradas. Itens já lançados não mudam.
                    </p>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">Descrição</label>
                        <input
                            type="text"
                            value={formData.description}
                            onChange={(e) => setFormData(prev => ({ ...prev, description: e.target.value }))}
                            required
                            maxLength={150}
                            className={inputClass}
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">Valor (R$)</label>
                        <ValueComponent
                            value={formData.value}
                            onChange={(value) => setFormData(prev => ({ ...prev, value }))}
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">Frequência</label>
                        <select
                            value={formData.frequencyType}
                            onChange={(e) => setFormData(prev => ({ ...prev, frequencyType: Number(e.target.value) as FrequencyType }))}
                            className={inputClass}
                        >
                            {Object.entries(FREQUENCY_LABELS).map(([value, label]) => (
                                <option key={value} value={value}>{label}</option>
                            ))}
                        </select>
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">Início</label>
                            <input
                                type="date"
                                value={formData.startDate}
                                onChange={(e) => setFormData(prev => ({ ...prev, startDate: e.target.value }))}
                                required
                                className={inputClass}
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Fim <span className="font-normal text-gray-600">(opcional)</span>
                            </label>
                            <input
                                type="date"
                                value={formData.endDate}
                                onChange={(e) => setFormData(prev => ({ ...prev, endDate: e.target.value }))}
                                className={inputClass}
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">Categoria</label>
                        <CategoryComboBox
                            value={formData.categoryId}
                            onChange={(categoryId) => setFormData(prev => ({ ...prev, categoryId }))}
                        />
                    </div>

                    <label className="flex items-center gap-2 text-xs font-semibold text-gray-300 cursor-pointer select-none">
                        <input
                            type="checkbox"
                            checked={formData.isActive}
                            onChange={(e) => setFormData(prev => ({ ...prev, isActive: e.target.checked }))}
                            className="w-4 h-4 accent-blue-500"
                        />
                        Assinatura ativa (gera itens nas próximas competências)
                    </label>

                    <div className="flex gap-2 pt-2">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 rounded border border-white/10 text-white hover:bg-white/5 transition text-sm font-medium cursor-pointer"
                        >
                            Cancelar
                        </button>
                        <button
                            type="submit"
                            disabled={isSubmitting}
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
