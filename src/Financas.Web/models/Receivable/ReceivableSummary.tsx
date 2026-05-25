export default class ReceivableSummaryView {
    id: string;
    description: string;
    total: number;
    date: string;

    constructor(id: string, description: string, total: number, date: string) {
        this.id = id;
        this.description = description;
        this.total = total;
        this.date = date;
    }

    static fromJSON(data: any): ReceivableSummaryView {
        return new ReceivableSummaryView(data.id, data.description, data.total, data.date);
    }
}
