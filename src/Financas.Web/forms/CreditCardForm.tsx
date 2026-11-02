import { useRef, useState } from 'react';
import { CreditCardRequest } from '@/services/creditCardServices';
import { ValueComponent } from '@/public/components/ValueComponent';

interface CreditCardFormProps {
    initialData?: {
        id: string;
        name: string;
        closingDay: number;
        dueDay: number;
        limit: number;
    } | null;
    onSubmit: (data: CreditCardRequest) => Promise<void>;
    onClose: () => void;
    isLoading?: boolean;
}

export default function CreditCardForm({
    initialData,
    onSubmit,
    onClose,
    isLoading = false,
}: CreditCardFormProps) {
    const [formData, setFormData] = useState({
        name: initialData?.name || '',
        closingDay: initialData?.closingDay || 10,
        dueDay: initialData?.dueDay || 20,
        limit: initialData?.limit || 0,
    });

    const [error, setError] = useState<string | null>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const formRef = useRef<HTMLFormElement>(null);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value, type } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'number' ? parseInt(value) || 0 : value,
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (formData.closingDay < 1 || formData.closingDay > 31) {
            setError("O dia de fechamento deve ser entre 1 e 31.");
            return;
        }

        if (formData.dueDay < 1 || formData.dueDay > 31) {
            setError("O dia de vencimento deve ser entre 1 e 31.");
            return;
        }

        setIsSubmitting(true);
        try {
            await onSubmit(formData);
            onClose();
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Erro ao salvar cartão');
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-[60] backdrop-blur-sm">
            <div className="bg-[#1a1917] rounded-xl border border-white/10 w-96 shadow-2xl overflow-hidden animate-in fade-in zoom-in duration-200">
                <div className="p-4 border-b border-white/5 flex justify-between items-center bg-[#151413]">
                    <h3 className="font-semibold text-sm uppercase tracking-wider text-blue-400">
                        {initialData ? 'Editar Cartão' : 'Novo Cartão'}
                    </h3>
                    <button
                        onClick={onClose}
                        className="text-gray-500 hover:text-white transition cursor-pointer"
                    >
                        ✕
                    </button>
                </div>

                <form ref={formRef} onSubmit={handleSubmit} className="p-5 space-y-4">
                    {error && (
                        <div className="p-3 bg-red-900/20 border border-red-500/30 rounded text-red-400 text-sm">
                            {error}
                        </div>
                    )}

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Nome do Cartão (ex: Nubank, Inter)
                        </label>
                        <input
                            type="text"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            required
                            className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                            placeholder="Nubank, Inter, etc."
                            tabIndex={1}
                        />
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Dia de Fechamento
                            </label>
                            <input
                                type="number"
                                name="closingDay"
                                min={1}
                                max={31}
                                value={formData.closingDay}
                                onChange={handleChange}
                                required
                                className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                                tabIndex={2}
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-semibold text-gray-400 mb-2">
                                Dia de Vencimento
                            </label>
                            <input
                                type="number"
                                name="dueDay"
                                min={1}
                                max={31}
                                value={formData.dueDay}
                                onChange={handleChange}
                                required
                                className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
                                tabIndex={3}
                            />
                        </div>
                    </div>

                    <div>
                        <label className="block text-xs font-semibold text-gray-400 mb-2">
                            Limite de Crédito (R$)
                        </label>
                        <ValueComponent
                            value={formData.limit}
                            onChange={(limit) => setFormData(prev => ({ ...prev, limit }))}
                            required
                            tabIndex={4}
                        />
                    </div>

                    <div className="flex gap-2 pt-4">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 rounded border border-white/10 text-white hover:bg-white/5 transition text-sm font-medium cursor-pointer"
                            tabIndex={5}
                        >
                            Cancelar
                        </button>
                        <button
                            type="submit"
                            disabled={isSubmitting || isLoading}
                            className="flex-1 px-4 py-2 rounded bg-blue-700 hover:bg-blue-600 disabled:bg-blue-900/50 disabled:cursor-not-allowed text-white transition text-sm font-medium cursor-pointer"
                            tabIndex={6}
                        >
                            {isSubmitting ? 'Salvando...' : 'Salvar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
