import { useRef, useState } from 'react';
import { CardExpenseItemRequest } from '@/services/cardExpenseServices';
import { DateComponent } from '@/public/components/DateComponent';
import { ValueComponent } from '@/public/components/ValueComponent';
import { SaveButton, SaveMode } from '@/public/components/SaveButton';
import { toInputDate } from '@/utils/dateFormat';

interface CardExpenseItemFormProps {
    cardExpenseId: string;
    initialData?: {
        id: string;
        description: string;
        total: number;
        dueDate: string;
        subTotal?: number;
        discounts: number;
        observation: string;
        currentInstallment?: number;
        currentInstallments?: number;
        quantityInstallments: number;
    } | null;
    onSubmit: (data: CardExpenseItemRequest) => Promise<void>;
    onClose: () => void;
    isLoading?: boolean;
}

export default function CardExpenseItemForm({
    cardExpenseId,
    initialData,
    onSubmit,
    onClose,
    isLoading = false,
}: CardExpenseItemFormProps) {
    const [formData, setFormData] = useState({
        description: initialData?.description || '',
        dueDate: toInputDate(initialData?.dueDate) || new Date().toISOString().split('T')[0],
        subTotal: initialData?.subTotal ?? initialData?.total ?? 0,
        discounts: initialData?.discounts || 0,
        observation: initialData?.observation || '',
        currentInstallment: initialData?.currentInstallment ?? initialData?.currentInstallments ?? 1,
        quantityInstallments: initialData?.quantityInstallments || 1,
    });
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [saveMode, setSaveMode] = useState<SaveMode>('close');
    const formRef = useRef<HTMLFormElement>(null);

    const isNew = !initialData;

    const getEmptyForm = () => ({
        description: '',
        dueDate: new Date().toISOString().split('T')[0],
        subTotal: 0,
        discounts: 0,
        observation: '',
        currentInstallment: 1,
        quantityInstallments: 1,
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: ['currentInstallment', 'quantityInstallments'].includes(name)
                ? parseInt(value, 10) || 0
                : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setIsSubmitting(true);

        try {
            await onSubmit(formData);

            if (saveMode === 'continue' && isNew) {
                setFormData(getEmptyForm());
            } else {
                onClose();
            }
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Erro ao salvar item');
        } finally {
            setIsSubmitting(false);
        }
    };

    const requestSubmit = () => formRef.current?.requestSubmit();

    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-96 shadow-2xl max-h-screen overflow-y-auto">
                <div className="p-4 border-b border-white/5 flex justify-between items-center">
                    <h3 className="font-semibold text-sm uppercase tracking-wider">
                        {initialData ? 'Editar Item' : 'Novo Item'}
                    </h3>
                    <button
                        onClick={onClose}
                        className="text-gray-500 hover:text-white transition"
                    >
                        ✕
                    </button>
                </div>

                <form ref={formRef} onSubmit={handleSubmit} className="p-4 space-y-4">
                    {error && (
                        <div className="p-3 bg-red-900/20 border border-red-500/30 rounded text-red-400 text-sm">
                            {error}
                        </div>
                    )}

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Descrição
                        </label>
                        <input
                            type="text"
                            name="description"
                            value={formData.description}
                            onChange={handleChange}
                            required
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                            placeholder="Ex: Netflix"
                        />
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Valor (R$)
                            </label>
                            <ValueComponent
                                value={formData.subTotal}
                                onChange={(subTotal) => setFormData(prev => ({ ...prev, subTotal }))}
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Descontos (R$)
                            </label>
                            <ValueComponent
                                value={formData.discounts}
                                onChange={(discounts) => setFormData(prev => ({ ...prev, discounts }))}
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Data de Vencimento
                        </label>
                        <DateComponent
                            date={formData.dueDate}
                            onChange={(newDate) => setFormData(prev => ({ ...prev, dueDate: newDate }))}
                        />
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Parcela Atual
                            </label>
                            <input
                                type="number"
                                name="currentInstallment"
                                min="1"
                                value={formData.currentInstallment}
                                onChange={handleChange}
                                required
                                className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                                placeholder="1"
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Total de Parcelas
                            </label>
                            <input
                                type="number"
                                name="quantityInstallments"
                                min="1"
                                value={formData.quantityInstallments}
                                onChange={handleChange}
                                required
                                className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                                placeholder="1"
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Observação
                        </label>
                        <textarea
                            name="observation"
                            value={formData.observation}
                            onChange={handleChange}
                            rows={2}
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition resize-none"
                            placeholder="Opcional"
                        />
                    </div>

                    <div className="flex gap-2 pt-4">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 rounded border border-white/10 text-white hover:bg-white/5 transition text-sm font-medium"
                        >
                            Cancelar
                        </button>
                        {isNew ? (
                            <SaveButton
                                mode={saveMode}
                                onModeChange={setSaveMode}
                                onClick={requestSubmit}
                                isSubmitting={isSubmitting}
                                disabled={isLoading}
                                className="bg-blue-700 hover:bg-blue-600 disabled:bg-blue-900/50"
                            />
                        ) : (
                            <button
                                type="submit"
                                disabled={isSubmitting || isLoading}
                                className="flex-1 px-4 py-2 rounded bg-blue-700 hover:bg-blue-600 disabled:bg-blue-900/50 disabled:cursor-not-allowed text-white transition text-sm font-medium"
                            >
                                {isSubmitting ? 'Salvando...' : 'Salvar'}
                            </button>
                        )}
                    </div>
                </form>
            </div>
        </div>
    );
}
