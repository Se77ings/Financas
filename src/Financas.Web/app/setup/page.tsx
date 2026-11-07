"use client"

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import CreditCardForm from "@/forms/CreditCardForm";
import CreditCardView from "@/models/CreditCard/CreditCardView";
import { addCreditCardAsync, CreditCardRequest, getCreditCardsAsync } from "@/services/creditCardServices";
import { chooseBackupFolderAsync, completeFirstRunAsync, getDesktopStatusAsync } from "@/services/desktopServices";
import { addEffectiveDateAsync, getEffectiveDateListAsync } from "@/services/effectiveDateServices";
import { IS_DESKTOP } from "@/utils/appMode";

const STEP_COUNT = 3;
const MIN_YEAR = 2000;
const MAX_YEAR = 2100;

const MONTH_NAMES = ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'];

const PRIMARY_BUTTON = "px-4 py-2 rounded bg-blue-700 hover:bg-blue-600 disabled:bg-blue-900/50 disabled:cursor-not-allowed text-white transition text-sm font-medium cursor-pointer";
const SECONDARY_BUTTON = "px-4 py-2 rounded border border-white/10 text-white hover:bg-white/5 disabled:opacity-50 disabled:cursor-not-allowed transition text-sm font-medium cursor-pointer";
const INPUT = "w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white focus:outline-none focus:border-blue-500 transition";

type OnError = (message: string | null) => void;

function errorMessage(err: unknown, fallback: string): string {
    return err instanceof Error ? err.message : fallback;
}

function BackupStep({ folder, onFolderChange, onError, onNext }: {
    folder: string | null;
    onFolderChange: (folder: string | null) => void;
    onError: OnError;
    onNext: () => void;
}) {
    const [isChoosing, setIsChoosing] = useState(false);

    const handleChoose = async () => {
        setIsChoosing(true);
        onError(null);
        try {
            const backup = await chooseBackupFolderAsync();
            onFolderChange(backup.folder);
        } catch (err) {
            onError(errorMessage(err, "Não foi possível escolher a pasta."));
        } finally {
            setIsChoosing(false);
        }
    };

    return (
        <>
            <div className="space-y-3 text-sm text-gray-300">
                <h2 className="text-lg font-semibold text-white">Onde guardar seus backups?</h2>
                <p>O backup é uma cópia dos seus dados. Com ele, você não perde nada se o computador der problema.</p>
                <p>Escolha uma pasta. Se ela estiver no OneDrive ou no Google Drive, a cópia também fica guardada na internet.</p>
                <p className="px-3 py-2 bg-black/20 border border-white/5 rounded text-gray-200 break-all">
                    {folder ?? "Nenhuma pasta escolhida"}
                </p>
            </div>
            <div className="flex justify-between gap-2 pt-6">
                <button onClick={handleChoose} disabled={isChoosing} className={SECONDARY_BUTTON}>
                    {isChoosing ? "Aguarde..." : folder ? "Trocar pasta" : "Escolher pasta"}
                </button>
                <button onClick={onNext} disabled={!folder || isChoosing} className={PRIMARY_BUTTON}>
                    Continuar
                </button>
            </div>
        </>
    );
}

function CardStep({ onError, onBack, onNext }: {
    onError: OnError;
    onBack: () => void;
    onNext: () => void;
}) {
    const [cards, setCards] = useState<CreditCardView[]>([]);
    const [showForm, setShowForm] = useState(false);

    useEffect(() => {
        getCreditCardsAsync()
            .then(setCards)
            .catch(err => onError(errorMessage(err, "Não foi possível carregar os cartões.")));
    }, [onError]);

    const handleSave = async (data: CreditCardRequest) => {
        await addCreditCardAsync(data);
        setCards(await getCreditCardsAsync());
    };

    return (
        <>
            <div className="space-y-3 text-sm text-gray-300">
                <h2 className="text-lg font-semibold text-white">Você usa cartão de crédito?</h2>
                <p>Cadastre um cartão para acompanhar as faturas. Se preferir, faça isso depois.</p>
                {cards.length > 0 && (
                    <ul className="px-3 py-2 bg-black/20 border border-white/5 rounded text-gray-200 space-y-1">
                        {cards.map(card => <li key={card.id}>💳 {card.name}</li>)}
                    </ul>
                )}
                <button onClick={() => setShowForm(true)} className={SECONDARY_BUTTON}>
                    {cards.length > 0 ? "Adicionar outro cartão" : "Adicionar cartão"}
                </button>
            </div>
            <div className="flex justify-between gap-2 pt-6">
                <button onClick={onBack} className={SECONDARY_BUTTON}>Voltar</button>
                <button onClick={onNext} className={PRIMARY_BUTTON}>
                    {cards.length > 0 ? "Continuar" : "Pular"}
                </button>
            </div>

            {showForm && <CreditCardForm onSubmit={handleSave} onClose={() => setShowForm(false)} />}
        </>
    );
}

function EffectiveDateStep({ onError, onBack }: {
    onError: OnError;
    onBack: () => void;
}) {
    const [month, setMonth] = useState(() => new Date().getMonth() + 1);
    const [year, setYear] = useState(() => new Date().getFullYear());
    const [isFinishing, setIsFinishing] = useState(false);

    const handleFinish = async () => {
        setIsFinishing(true);
        onError(null);
        try {
            // Uma tentativa anterior pode ter criado o mês e falhado depois, e a API recusa mês repetido.
            const existing = await getEffectiveDateListAsync();
            if (!existing.some(d => d.month === month && d.year === year)) {
                await addEffectiveDateAsync({ month, year });
            }
            await completeFirstRunAsync();
            // Recarrega a página porque a barra inferior fica no layout e não enxergaria o mês criado aqui.
            window.location.replace('/');
        } catch (err) {
            onError(errorMessage(err, "Não foi possível concluir."));
            setIsFinishing(false);
        }
    };

    return (
        <>
            <div className="space-y-3 text-sm text-gray-300">
                <h2 className="text-lg font-semibold text-white">Por qual mês quer começar?</h2>
                <p>Suas contas e recebimentos ficam organizados por mês. No programa, cada mês se chama competência.</p>
                <div className="grid grid-cols-2 gap-3">
                    <label className="block text-xs font-semibold text-gray-400">
                        <span className="block mb-2">Mês</span>
                        <select value={month} onChange={e => setMonth(Number(e.target.value))} className={INPUT}>
                            {MONTH_NAMES.map((name, index) => (
                                <option key={name} value={index + 1}>{name}</option>
                            ))}
                        </select>
                    </label>
                    <label className="block text-xs font-semibold text-gray-400">
                        <span className="block mb-2">Ano</span>
                        <input
                            type="number"
                            min={MIN_YEAR}
                            max={MAX_YEAR}
                            value={year || ''}
                            onChange={e => setYear(parseInt(e.target.value) || 0)}
                            className={INPUT}
                        />
                    </label>
                </div>
            </div>
            <div className="flex justify-between gap-2 pt-6">
                <button onClick={onBack} disabled={isFinishing} className={SECONDARY_BUTTON}>Voltar</button>
                <button
                    onClick={handleFinish}
                    disabled={isFinishing || year < MIN_YEAR || year > MAX_YEAR}
                    className={PRIMARY_BUTTON}
                >
                    {isFinishing ? "Concluindo..." : "Concluir"}
                </button>
            </div>
        </>
    );
}

export default function SetupPage() {
    const router = useRouter();
    const [step, setStep] = useState(1);
    const [folder, setFolder] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!IS_DESKTOP) {
            router.replace('/');
            return;
        }

        getDesktopStatusAsync()
            .then(status => {
                if (!status.firstRunPending) {
                    router.replace('/');
                    return;
                }
                setFolder(status.backup.folder);
                setIsLoading(false);
            })
            .catch(err => {
                setError(errorMessage(err, "Não foi possível abrir a configuração."));
                setIsLoading(false);
            });
    }, [router]);

    const goToStep = (target: number) => {
        setError(null);
        setStep(target);
    };

    if (isLoading) return <div className="flex-1 bg-blue-950" />;

    return (
        <div className="flex-1 min-h-0 overflow-auto flex p-5 text-gray-100 bg-blue-950">
            <div className="m-auto bg-[#1a1917] rounded-xl border border-white/5 w-[520px] max-w-full shadow-2xl overflow-hidden">
                <div className="p-4 border-b border-white/5 flex justify-between items-center bg-[#151413]">
                    <h1 className="font-bold text-sm uppercase tracking-wider text-blue-400">Bem-vindo ao Finanças Pessoais</h1>
                    <span className="text-xs text-gray-500 shrink-0">Passo {step} de {STEP_COUNT}</span>
                </div>

                <div className="p-5">
                    {error && (
                        <div className="p-3 mb-4 bg-red-900/20 border border-red-500/30 rounded text-red-400 text-sm">
                            {error}
                        </div>
                    )}

                    {step === 1 && (
                        <BackupStep folder={folder} onFolderChange={setFolder} onError={setError} onNext={() => goToStep(2)} />
                    )}
                    {step === 2 && (
                        <CardStep onError={setError} onBack={() => goToStep(1)} onNext={() => goToStep(3)} />
                    )}
                    {step === 3 && (
                        <EffectiveDateStep onError={setError} onBack={() => goToStep(2)} />
                    )}
                </div>
            </div>
        </div>
    );
}
