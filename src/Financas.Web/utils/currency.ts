const MAX_CENTS = 999_999_999_99;

export function valueToCents(value: number): number {
    if (!Number.isFinite(value) || value < 0) return 0;
    return Math.min(Math.round(value * 100), MAX_CENTS);
}

export function centsToValue(cents: number): number {
    return cents / 100;
}

export function formatCurrencyBR(cents: number): string {
    return (cents / 100).toLocaleString('pt-BR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    });
}

export { MAX_CENTS };
