/** Exibe data em dd/mm/yyyy (pt-BR). Aceita ISO, yyyy-mm-dd ou dd/mm/yyyy. */
export function formatDateBR(value: string | null | undefined): string {
    if (!value || !value.trim()) return '-';

    const trimmed = value.trim();
    const brMatch = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(trimmed);
    if (brMatch) return trimmed;

    const isoDate = trimmed.includes('T') ? trimmed.split('T')[0] : trimmed;
    const isoMatch = /^(\d{4})-(\d{2})-(\d{2})$/.exec(isoDate);
    if (isoMatch) {
        const [, year, month, day] = isoMatch;
        return `${day}/${month}/${year}`;
    }

    const parsed = new Date(trimmed);
    if (!Number.isNaN(parsed.getTime())) {
        return parsed.toLocaleDateString('pt-BR', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
        });
    }

    return trimmed;
}

/** Converte para yyyy-mm-dd (input type="date"). */
export function toInputDate(value: string | null | undefined): string {
    if (!value || !value.trim()) return '';

    const trimmed = value.trim();
    const brMatch = /^(\d{2})\/(\d{2})\/(\d{4})$/.exec(trimmed);
    if (brMatch) {
        const [, day, month, year] = brMatch;
        return `${year}-${month}-${day}`;
    }

    if (/^\d{4}-\d{2}-\d{2}/.test(trimmed)) {
        return trimmed.slice(0, 10);
    }

    const parsed = new Date(trimmed);
    if (!Number.isNaN(parsed.getTime())) {
        const y = parsed.getFullYear();
        const m = String(parsed.getMonth() + 1).padStart(2, '0');
        const d = String(parsed.getDate()).padStart(2, '0');
        return `${y}-${m}-${d}`;
    }

    return '';
}

/** Hoje em yyyy-mm-dd no fuso do computador: o toISOString usa UTC e, à noite no Brasil, já devolve o dia seguinte. */
export function todayInputDate(): string {
    const now = new Date();
    const m = String(now.getMonth() + 1).padStart(2, '0');
    const d = String(now.getDate()).padStart(2, '0');
    return `${now.getFullYear()}-${m}-${d}`;
}
