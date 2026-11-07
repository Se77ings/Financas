import { useRef, useState } from 'react';
import { CardExpenseItemRequest } from '@/services/cardExpenseServices';
import { DateComponent } from '@/public/components/DateComponent';
import { ValueComponent } from '@/public/components/ValueComponent';
import { SaveButton } from '@/public/components/SaveButton';
import { useSaveMode } from '@/public/components/useSaveMode';
import { CategoryComboBox } from '@/public/components/CategoryComboBox';
import { useCategorySuggestion } from '@/public/components/useCategorySuggestion';
import { rememberCategorizedDescription } from '@/services/categorySuggestionServices';
import { todayInputDate, toInputDate } from '@/utils/dateFormat';

interface CardExpenseItemFormProps {
    cardExpenseId?: string;
    creditCardId?: string | null;
    initialData?: {
        id: string;
        description: string;
        total: number;
        dueDate: string;
        subTotal?: number;
        discounts: number;
        observation: string;
        currentInstallment?: number;
        quantityInstallments: number;
        categoryId?: string | null;
    } | null;
    onSubmit: (data: CardExpenseItemRequest) => Promise<void>;
    onClose: () => void;
    isLoading?: boolean;
    lockRecurring?: boolean;
}

export default function CardExpenseItemForm({
    cardExpenseId,
    creditCardId,
    initialData,
    onSubmit,
    onClose,
    isLoading = false,
    lockRecurring = false,
}: CardExpenseItemFormProps) {
    const [formData, setFormData] = useState({
        description: initialData?.description || '',
        dueDate: toInputDate(initialData?.dueDate) || todayInputDate(),
        subTotal: initialData?.subTotal ?? initialData?.total ?? 0,
        discounts: initialData?.discounts || 0,
        observation: initialData?.observation || '',
        currentInstallment: initialData?.currentInstallment ?? 1,
        quantityInstallments: initialData?.quantityInstallments || 1,
        categoryId: initialData?.categoryId ?? null,
    });
    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [saveMode, setSaveMode] = useSaveMode(lockRecurring ? 'card-expense-item:recurring' : 'card-expense-item');
    const formRef = useRef<HTMLFormElement>(null);

    const [isRecurring, setIsRecurring] = useState(lockRecurring);
    const [recurrenceData, setRecurrenceData] = useState({
        frequencyType: '1',
        startDate: formData.dueDate,
        endDate: '',
    });

    const isNew = !initialData;
    const categorySuggestion = useCategorySuggestion(
        formData.description,
        formData.categoryId,
        (categoryId) => setFormData(prev => ({ ...prev, categoryId })),
    );

    const getEmptyForm = (currentDate: string) => ({
        description: '',
        dueDate: currentDate,
        subTotal: 0,
        discounts: 0,
        observation: '',
        currentInstallment: 1,
        quantityInstallments: 1,
        categoryId: null,
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

        if (!formData.description.trim()) {
            setError('A descrição deve ser informada.');
            return;
        }

        if (formData.description.length > 100) {
            setError('A descrição deve ter no máximo 100 caracteres.');
            return;
        }

        if (!formData.dueDate) {
            setError('A data de vencimento não pode ser vazia.');
            return;
        }

        if (formData.discounts < 0) {
            setError('Os descontos não podem ser negativos.');
            return;
        }

        if (formData.currentInstallment < 1 || formData.quantityInstallments < 1) {
            setError('A parcela atual e a quantidade de parcelas devem ser no mínimo 1.');
            return;
        }

        if (formData.currentInstallment > formData.quantityInstallments) {
            setError('A parcela atual não pode ser maior que a quantidade de parcelas.');
            return;
        }

        if (formData.subTotal <= 0) {
            setError('O valor deve ser maior que zero.');
            return;
        }

        if (!formData.categoryId) {
            setError('Selecione uma categoria.');
            return;
        }

        if (isRecurring && !recurrenceData.startDate) {
            setError('A data de início é obrigatória para assinaturas.');
            return;
        }

        if (isRecurring && recurrenceData.endDate && recurrenceData.endDate < recurrenceData.startDate) {
            setError('A data de término não pode ser anterior à data de início.');
            return;
        }

        setIsSubmitting(true);

        try {
            const submitData: CardExpenseItemRequest = {
                ...formData,
                isRecurrent: isRecurring,
                ...(isRecurring ? {
                    recurrenceFrequencyType: parseInt(recurrenceData.frequencyType, 10),
                    recurrenceStartDate: recurrenceData.startDate,
                    recurrenceEndDate: recurrenceData.endDate || null,
                } : {})
            };

            await onSubmit(submitData);

            rememberCategorizedDescription(formData.description, formData.categoryId);

            if (saveMode === 'continue' && isNew) {
                setFormData(getEmptyForm(formData.dueDate));
                categorySuggestion.reset();
                setIsRecurring(lockRecurring);
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
            <div className="bg-[#1a1917] rounded-xl border border-white/5 w-96 shadow-2xl max-h-screen overflow-y-auto" style={{ marginRight: '50vh' }}>
                <div className="p-4 border-b border-white/5 flex justify-between items-center">
                    <h3 className="font-semibold text-sm uppercase tracking-wider">
                        {initialData ? 'Editar Item' : lockRecurring ? 'Nova Assinatura' : 'Novo Item'}
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
                            tabIndex={1}
                        />
                    </div>

                    <div className={`grid ${formData.discounts !== 0 ? 'grid-cols-3' : 'grid-cols-2'} gap-3`}>
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Subtotal (R$)
                            </label>
                            <ValueComponent
                                value={formData.subTotal}
                                onChange={(subTotal) => setFormData(prev => ({ ...prev, subTotal }))}
                                required
                                tabIndex={2}
                            />
                        </div>
                        <div className={isRecurring ? "opacity-50 pointer-events-none" : ""}>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Descontos (R$)
                            </label>
                            <ValueComponent
                                tabIndex={-1}
                                value={formData.discounts}
                                onChange={(discounts) => !isRecurring && setFormData(prev => ({ ...prev, discounts }))}
                            />
                        </div>
                        {formData.discounts !== 0 && (
                            <div>
                                <label className="block text-xs font-semibold text-gray-400 mb-2">
                                    Total (R$)
                                </label>
                                <input
                                    type="text"
                                    readOnly
                                    disabled
                                    value={(formData.subTotal - formData.discounts).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}
                                    className="w-full px-3 py-2 bg-[#0f0e0c]/50 border border-white/5 rounded text-gray-400 font-mono text-sm focus:outline-none cursor-not-allowed"
                                />
                            </div>
                        )}
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Data
                        </label>
                        <DateComponent
                            plustabindex={3}
                            minustabindex={-1}
                            tabIndex={-1}
                            date={formData.dueDate}
                            onChange={(newDate) => setFormData(prev => ({ ...prev, dueDate: newDate }))}
                        />
                    </div>

                    {!isRecurring && (
                        <div className="grid grid-cols-2 gap-3">
                            <div>
                                <label className="block text-xs font-semibold text-gray-400 mb-2">
                                    Parcela Atual
                                </label>
                                <input tabIndex={-1}
                                    type="number"
                                    name="currentInstallment"
                                    min="1"
                                    value={formData.currentInstallment}
                                    onChange={handleChange}
                                    required={!isRecurring}
                                    className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                                    placeholder="1"
                                />
                            </div>
                            <div>
                                <label className="block text-xs font-semibold text-gray-400 mb-2">
                                    Total de Parcelas
                                </label>
                                <input
                                    tabIndex={-1}
                                    type="number"
                                    name="quantityInstallments"
                                    min="1"
                                    value={formData.quantityInstallments}
                                    onChange={handleChange}
                                    required={!isRecurring}
                                    className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                                    placeholder="1"
                                />
                            </div>
                        </div>
                    )}

                    {isNew && creditCardId && (
                        <div className="border-t border-white/5 pt-4 space-y-3">
                            <div className="flex items-center justify-between">
                                <span className="text-xs font-semibold text-gray-400">Tornar este item recorrente (Assinatura)?</span>
                                <label className={`relative inline-flex items-center ${lockRecurring ? "cursor-not-allowed opacity-70" : "cursor-pointer"}`}>
                                    <input
                                        type="checkbox"
                                        checked={isRecurring}
                                        disabled={lockRecurring}
                                        onChange={(e) => {
                                            const checked = e.target.checked;
                                            setIsRecurring(checked);
                                            if (checked) {
                                                setFormData(prev => ({
                                                    ...prev,
                                                    currentInstallment: 1,
                                                    quantityInstallments: 1,
                                                    discounts: 0
                                                }));
                                            }
                                        }}
                                        className="sr-only peer"
                                    />
                                    <div className="w-9 h-5 bg-[#0f0e0c] peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-gray-500 after:border-gray-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-blue-600 peer-checked:after:bg-white"></div>
                                </label>
                            </div>

                            {isRecurring && (
                                <div className="p-3 bg-[#0f0e0c]/40 border border-white/5 rounded space-y-3 transition-all duration-300">
                                    <div>
                                        <label className="block text-xs font-semibold text-gray-400 mb-1">Frequência</label>
                                        <select
                                            value={recurrenceData.frequencyType}
                                            onChange={(e) => setRecurrenceData(prev => ({ ...prev, frequencyType: e.target.value }))}
                                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white focus:outline-none focus:border-blue-500 text-xs"
                                        >
                                            <option value="1">Mensal</option>
                                            <option value="2">Trimestral</option>
                                            <option value="3">Semestral</option>
                                            <option value="4">Anual</option>
                                        </select>
                                    </div>
                                    <div className="grid grid-cols-2 gap-2">
                                        <div>
                                            <label className="block text-xs font-semibold text-gray-400 mb-1">Início</label>
                                            <input
                                                type="date"
                                                value={recurrenceData.startDate}
                                                onChange={(e) => setRecurrenceData(prev => ({ ...prev, startDate: e.target.value }))}
                                                className="w-full px-2 py-1 bg-[#0f0e0c] border border-white/10 rounded text-white focus:outline-none focus:border-blue-500 text-xs"
                                            />
                                        </div>
                                        <div>
                                            <label className="block text-xs font-semibold text-gray-400 mb-1">Fim (Opcional)</label>
                                            <input
                                                type="date"
                                                value={recurrenceData.endDate}
                                                onChange={(e) => setRecurrenceData(prev => ({ ...prev, endDate: e.target.value }))}
                                                className="w-full px-2 py-1 bg-[#0f0e0c] border border-white/10 rounded text-white focus:outline-none focus:border-blue-500 text-xs"
                                            />
                                        </div>
                                    </div>
                                </div>
                            )}
                        </div>
                    )}

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Categoria
                            {categorySuggestion.isSuggested && (
                                <span className="ml-2 font-normal text-amber-400" title="Sugerida pelas palavras da descrição. Você pode trocar.">✨ sugerida</span>
                            )}
                        </label>
                        <CategoryComboBox
                            value={formData.categoryId}
                            onChange={categorySuggestion.selectManually}
                            tabIndex={3}
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
                            rows={2}
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition resize-none"
                            placeholder="Opcional"
                            tabIndex={-1}
                        />
                    </div>

                    <div className="flex gap-2 pt-4">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 rounded border border-white/10 text-white hover:bg-white/5 transition text-sm font-medium"
                            tabIndex={-1}    
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
                                tabIndex={4}
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
