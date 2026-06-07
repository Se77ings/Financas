import { getAsync, postAsync } from "./baseServices";
import EffectiveDateSummary from '@/models/EffectiveDate/EffectiveDateSummary';
import EffectiveDateView from "@/models/EffectiveDate/EffectiveDateView";
import { Deserializer } from "./Deserializer";

export async function getEffectiveDateByIdAsync(id: string): Promise<EffectiveDateView> {
    const response = await getAsync(`api/effectiveDate/${id}`);

    const data = await response.json();
    
    return Deserializer.deserialize(data, EffectiveDateView);
}

export async function getEffectiveDateListAsync(): Promise<EffectiveDateSummary[]> {
    const response = await getAsync(`api/effectiveDate`);
    const data = await response.json();
    
    return Deserializer.deserializeArray(data, EffectiveDateSummary);
}

export async function syncMissingCardInvoicesAsync(id: string): Promise<void> {
    const response = await postAsync(`api/effectiveDate/${id}/sync-cards`, {});
    
    if (!response.ok) {
        const error = await response.json().catch(() => ({ message: 'Failed to sync cards' }));
        throw new Error(error.message || 'Failed to sync cards');
    }
}

export async function addEffectiveDateAsync(request: { month: number, year: number }): Promise<void> {
    const response = await postAsync(`api/effectiveDate`, request);
    
    if (!response.ok) {
        const error = await response.json().catch(() => ({ message: 'Failed to add competency' }));
        throw new Error(error.message || 'Failed to add competency');
    }
}

export async function getEligibleEffectiveDatesForTransferAsync(): Promise<EffectiveDateSummary[]> {
    const response = await getAsync(`api/effectiveDate/eligible-for-transfer`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch eligible competencies: ${response.statusText}`);
    }
    
    const data = await response.json();
    return Deserializer.deserializeArray(data, EffectiveDateSummary);
}