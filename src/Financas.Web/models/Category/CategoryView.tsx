export default class CategoryView {
    id: string;
    name: string;
    description: string | null;
    entriesCount: number;

    constructor(id: string, name: string, description: string | null, entriesCount: number) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.entriesCount = entriesCount;
    }

    static fromJSON(data: any): CategoryView {
        return new CategoryView(data.id, data.name, data.description ?? null, data.entriesCount ?? 0);
    }
}
