import { useRef, useState } from 'react';
import { CardExpenseRequest } from '@/services/cardExpenseServices';
import { DateComponent } from '@/public/components/DateComponent';
import { ValueComponent } from '@/public/components/ValueComponent';
import { SaveButton, SaveMode } from '@/public/components/SaveButton';
import { toInputDate } from '@/utils/dateFormat';

interface CardExpenseFormProps {
    effectiveDateId: string;
    initialData?: {
        id: string;
        description: string;
        total: number;
        dueDate: string;
        subtotal: number;
        discounts: number;
        observation: string;
    } | null;
    onSubmit: (data: CardExpenseRequest) => Promise<void>;
    onClose: () => void;
    isLoading?: boolean;
}

export default function CardExpenseForm({
    effectiveDateId,
    initialData,
    onSubmit,
    onClose,
    isLoading = false,
}: CardExpenseFormProps) {
    const [formData, setFormData] = useState({
        description: initialData?.description || '',
        dueDate: toInputDate(initialData?.dueDate) || new Date().toISOString().split('T')[0],
        subTotal: initialData?.subtotal ?? initialData?.total ?? 0,
        discounts: initialData?.discounts || 0,
        observation: initialData?.observation || '',
    });

    const computedTotal = Math.max(0, formData.subTotal - formData.discounts);
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [saveMode, setSaveMode] = useState<SaveMode>('close');
    const formRef = useRef<HTMLFormElement>(null);

    const isNew = !initialData;

    const getEmptyForm = (currentDate: string) => ({
        description: '',
        dueDate: currentDate,
        subTotal: 0,
        discounts: 0,
        observation: '',
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setIsSubmitting(true);

        try {
            await onSubmit({
                ...formData,
                effectiveDateId,
            });

            if (saveMode === 'continue' && isNew) {
                setFormData(getEmptyForm(formData.dueDate));
            } else {
                onClose();
            }
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Erro ao salvar faturas');
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
                        {initialData ? 'Editar Cartão' : 'Novo Cartão'}
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
                            Nome do Cartão
                        </label>
                        <input
                            tabIndex={0}
                            type="text"
                            name="description"
                            value={formData.description}
                            onChange={handleChange}
                            required
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                            placeholder="Ex: Inter"
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
                                tabIndex={2}
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Descontos (R$)
                            </label>
                            <ValueComponent
                                value={formData.discounts}
                                onChange={(discounts) => setFormData(prev => ({ ...prev, discounts }))}
                                tabIndex={1}
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Total (R$)
                        </label>
                        <ValueComponent value={computedTotal} onChange={() => {}} readOnly />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Data de Vencimento
                        </label>
                        <DateComponent
                            date={formData.dueDate}
                            onChange={(newDate) => setFormData(prev => ({ ...prev, dueDate: newDate }))}
                            plustabindex={0}
                            minustabindex={9}
                            tabIndex={1}
                            
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Observação
                        </label>
                        <textarea
                            name="observation"
                            value={formData.observation}
                            onChange={handleChange}
                            rows={3}
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition resize-none"
                            placeholder="Opcional"
                            tabIndex={7}
                        />
                    </div>

                    <div className="flex gap-2 pt-4">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 rounded border border-white/10 text-white hover:bg-white/5 transition text-sm font-medium cursor-pointer"
                            tabIndex={8}
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
                                tabIndex={9}
                            />
                        ) : (
                            <button
                                type="submit"
                                disabled={isSubmitting || isLoading}
                                className="flex-1 px-4 py-2 rounded bg-blue-700 hover:bg-blue-600 disabled:bg-blue-900/50 disabled:cursor-not-allowed text-white transition text-sm font-medium cursor-pointer"
                                tabIndex={0}
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
