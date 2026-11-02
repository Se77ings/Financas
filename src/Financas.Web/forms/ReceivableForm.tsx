import { useRef, useState } from 'react';
import { ReceivableRequest } from '@/services/receivableServices';
import { DateComponent } from '@/public/components/DateComponent';
import { ValueComponent } from '@/public/components/ValueComponent';
import { SaveButton } from '@/public/components/SaveButton';
import { useSaveMode } from '@/public/components/useSaveMode';
import { CategoryComboBox } from '@/public/components/CategoryComboBox';
import { toInputDate } from '@/utils/dateFormat';

interface ReceivableFormProps {
    effectiveDateId: string;
    initialData?: {
        id: string;
        description: string;
        subtotal?: number;
        total?: number;
        date: string;
        categoryId?: string | null;
    } | null;
    onSubmit: (data: ReceivableRequest) => Promise<void>;
    onClose: () => void;
    isLoading?: boolean;
}

export default function ReceivableForm({
    effectiveDateId,
    initialData,
    onSubmit,
    onClose,
    isLoading = false,
}: ReceivableFormProps) {
    const [formData, setFormData] = useState({
        description: initialData?.description || '',
        subtotal: initialData?.subtotal ?? initialData?.total ?? 0,
        date: toInputDate(initialData?.date) || new Date().toISOString().split('T')[0],
        categoryId: initialData?.categoryId ?? null,
    });
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [saveMode, setSaveMode] = useSaveMode('receivable');
    const formRef = useRef<HTMLFormElement>(null);

    const isNew = !initialData;

    const getEmptyForm = (currentDate: string) => ({
        description: '',
        subtotal: 0,
        date: currentDate,
        categoryId: null,
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (formData.subtotal <= 0) {
            setError('O valor deve ser maior que zero.');
            return;
        }

        if (!formData.categoryId) {
            setError('Selecione uma categoria.');
            return;
        }

        setIsSubmitting(true);

        try {
            await onSubmit({
                ...formData,
                effectiveDateId,
            });

            if (saveMode === 'continue' && isNew) {
                setFormData(getEmptyForm(formData.date));
            } else {
                onClose();
            }
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Erro ao salvar recebimento');
        } finally {
            setIsSubmitting(false);
        }
    };

    const requestSubmit = () => formRef.current?.requestSubmit();


    return (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-96 shadow-2xl">
                <div className="p-4 border-b border-white/5 flex justify-between items-center">
                    <h3 className="font-semibold text-sm uppercase tracking-wider">
                        {initialData ? 'Editar Recebimento' : 'Novo Recebimento'}
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
                            tabIndex={1}
                            type="text"
                            name="description"
                            value={formData.description}
                            onChange={handleChange}
                            required
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                            placeholder="Ex: Salário Mensal"
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Valor (R$)
                        </label>
                        <ValueComponent
                            value={formData.subtotal}
                            onChange={(subtotal) => setFormData(prev => ({ ...prev, subtotal }))}
                            required
                            tabIndex={2}
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Data
                        </label>
                        <DateComponent
                            date={formData.date}
                            onChange={(newDate) => setFormData(prev => ({ ...prev, date: newDate }))}
                            tabIndex={4}
                            plustabindex={5}
                            minustabindex={3}
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Categoria
                        </label>
                        <CategoryComboBox
                            value={formData.categoryId}
                            onChange={(categoryId) => setFormData(prev => ({ ...prev, categoryId }))}
                            tabIndex={5}
                        />
                    </div>

                    <div className="flex gap-2 pt-4">
                        <button
                            tabIndex={6}
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 rounded border border-white/10 text-white hover:bg-white/5 transition text-sm font-medium cursor-pointer"
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
                                tabIndex={7}
                            />
                        ) : (
                            <button
                                tabIndex={7}
                                type="submit"
                                disabled={isSubmitting || isLoading}
                                className="flex-1 px-4 py-2 rounded bg-green-700 hover:bg-green-600 disabled:bg-green-900/50 disabled:cursor-not-allowed text-white transition text-sm font-medium cursor-pointer"
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
