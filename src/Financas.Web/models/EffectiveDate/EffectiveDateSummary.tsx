import { formatEffectiveDateLabel } from '@/utils/calendarMonth';

export default class EffectiveDateSummary {
    id: string;
    month: number;
    year: number;
    display: string;
    receivableCount: number;
    payableCount: number;
    balance: number;

    constructor(id: string, month: number, year: number, receivableCount: number, payableCount: number, balance: number) {
        this.id = id;
        this.month = month;
        this.year = year;
        this.display = formatEffectiveDateLabel(month, year);
        this.receivableCount = receivableCount;
        this.payableCount = payableCount;
        this.balance = balance;
    }

    static fromJSON(data: any): EffectiveDateSummary {
        const month = typeof data.month === 'number' ? data.month : Number(data.month);
        const year = data.year;
        return new EffectiveDateSummary(
            data.id,
            month,
            year,
            data.receivableCount,
            data.payableCount,
            data.balance
        );
    }
}
