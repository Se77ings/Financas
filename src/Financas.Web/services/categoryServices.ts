import { getAsync, postAsync, putAsync, deleteAsync, readErrorAsync } from "./baseServices";
import CategoryView from "@/models/Category/CategoryView";
import EntriesByDescriptionView from "@/models/Entry/EntriesByDescriptionView";
import { Deserializer } from "./Deserializer";

export interface CategoryRequest {
    name: string;
    description: string | null;
}

export interface RenameEntriesRequest {
    payableIds: string[];
    receivableIds: string[];
    description: string;
}

export interface CategoryLinkRequest {
    ids: string[];
    categoryId: string;
}

export async function getCategoriesAsync(): Promise<CategoryView[]> {
    const response = await getAsync(`api/Category`);

    if (!response.ok) {
        throw new Error(`Failed to fetch categories: ${response.statusText}`);
    }

    const data = await response.json();
    return Deserializer.deserializeArray(data, CategoryView);
}

export async function addCategoryAsync(request: CategoryRequest): Promise<CategoryView> {
    const response = await postAsync(`api/Category`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to add category'));
    }

    const data = await response.json();
    return Deserializer.deserialize(data, CategoryView);
}

export async function updateCategoryAsync(id: string, request: CategoryRequest): Promise<void> {
    const response = await putAsync(`api/Category/${id}`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to update category'));
    }
}

export async function deleteCategoryAsync(id: string): Promise<void> {
    const response = await deleteAsync(`api/Category/${id}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to delete category'));
    }
}

export async function linkPayablesToCategoryAsync(request: CategoryLinkRequest): Promise<void> {
    const response = await putAsync(`api/Category/link/payables`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to link payables'));
    }
}

export async function linkReceivablesToCategoryAsync(request: CategoryLinkRequest): Promise<void> {
    const response = await putAsync(`api/Category/link/receivables`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to link receivables'));
    }
}

export async function getUncategorizedEntriesAsync(): Promise<EntriesByDescriptionView[]> {
    const response = await getAsync(`api/Category/uncategorized/entries`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to fetch uncategorized entries'));
    }

    const data = await response.json();
    return Deserializer.deserializeArray(data, EntriesByDescriptionView);
}

export async function getEntriesByCategoryAsync(categoryId: string): Promise<EntriesByDescriptionView[]> {
    const response = await getAsync(`api/Category/${categoryId}/entries`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to fetch category entries'));
    }

    const data = await response.json();
    return Deserializer.deserializeArray(data, EntriesByDescriptionView);
}

export async function renameEntriesAsync(request: RenameEntriesRequest): Promise<void> {
    const response = await putAsync(`api/Category/entries/description`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to rename entries'));
    }
}
