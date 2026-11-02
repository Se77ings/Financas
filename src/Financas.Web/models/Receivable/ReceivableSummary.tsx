export default class ReceivableSummaryView {
    id: string;
    description: string;
    total: number;
    date: string;
    categoryId: string | null;

    constructor(id: string, description: string, total: number, date: string, categoryId: string | null) {
        this.id = id;
        this.description = description;
        this.total = total;
        this.date = date;
        this.categoryId = categoryId;
    }

    static fromJSON(data: any): ReceivableSummaryView {
        return new ReceivableSummaryView(data.id, data.description, data.total, data.date, data.categoryId ?? null);
    }
}
