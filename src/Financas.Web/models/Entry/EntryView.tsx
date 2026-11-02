export type EntryType = 'ImmediateExpense' | 'CardExpenseItem' | 'Receivable';

export default class EntryView {
    id: string;
    type: EntryType;
    description: string;
    total: number;
    date: string;
    month: number;
    year: number;
    creditCardName: string | null;
    currentInstallment: number | null;
    quantityInstallments: number | null;
    observation: string | null;

    constructor(id: string, type: EntryType, description: string, total: number, date: string, month: number, year: number, creditCardName: string | null, currentInstallment: number | null, quantityInstallments: number | null, observation: string | null) {
        this.id = id;
        this.type = type;
        this.description = description;
        this.total = total;
        this.date = date;
        this.month = month;
        this.year = year;
        this.creditCardName = creditCardName;
        this.currentInstallment = currentInstallment;
        this.quantityInstallments = quantityInstallments;
        this.observation = observation;
    }

    static fromJSON(data: any): EntryView {
        return new EntryView(data.id, data.type, data.description, data.total, data.date, Number(data.month), data.year, data.creditCardName ?? null, data.currentInstallment ?? null, data.quantityInstallments ?? null, data.observation || null);
    }
}
