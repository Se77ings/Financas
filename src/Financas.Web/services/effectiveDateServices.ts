import { getAsync } from "./baseServices";
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