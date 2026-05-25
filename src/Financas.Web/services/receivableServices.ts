import { getAsync, postAsync, putAsync, deleteAsync } from "./baseServices";
import ReceivableSummaryView from "@/models/Receivable/ReceivableSummary";
import { Deserializer } from "./Deserializer";

export interface ReceivableRequest {
    description: string;
    subtotal: number;
    date: string;
    effectiveDateId: string;
}

export async function getReceivablesAsync(): Promise<ReceivableSummaryView[]> {
    const response = await getAsync(`api/Receivable`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch receivables: ${response.statusText}`);
    }

    const data = await response.json();
    
    return Deserializer.deserializeArray(data, ReceivableSummaryView);
}

export async function getReceivableByIdAsync(id: string): Promise<ReceivableSummaryView> {
    const response = await getAsync(`api/Receivable/${id}`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch receivable: ${response.statusText}`);
    }

    const data = await response.json();
    
    return Deserializer.deserialize(data, ReceivableSummaryView);
}

export async function addReceivableAsync(request: ReceivableRequest): Promise<void> {
    const response = await postAsync(`api/Receivable`, request);
    
    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to add receivable');
    }
}

export async function updateReceivableAsync(id: string, request: ReceivableRequest): Promise<void> {
    const response = await putAsync(`api/Receivable/${id}`, request);

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to update receivable');
    }
}

export async function deleteReceivableAsync(id: string): Promise<void> {
    const response = await deleteAsync(`api/Receivable/${id}`);

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to delete receivable');
    }
}
