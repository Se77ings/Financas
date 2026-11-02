import EntryView from "@/models/Entry/EntryView";

export default class EntriesByDescriptionView {
    description: string;
    count: number;
    total: number;
    entries: EntryView[];

    constructor(description: string, count: number, total: number, entries: EntryView[]) {
        this.description = description;
        this.count = count;
        this.total = total;
        this.entries = entries;
    }

    static fromJSON(data: any): EntriesByDescriptionView {
        return new EntriesByDescriptionView(
            data.description,
            data.count,
            data.total,
            (data.entries ?? []).map((e: any) => EntryView.fromJSON(e))
        );
    }
}
