import { useState } from 'react';
import { toInputDate } from '@/utils/dateFormat';

export function DateComponent({ date, onChange }: { date: string, onChange: (value: string) => void }) {
    const [currentValue, setCurrentValue] = useState(() => {
        const normalized = toInputDate(date);
        return normalized || new Date().toISOString().split('T')[0];
    });


    const updateValue = (newValue: string) => {
        setCurrentValue(newValue);
        onChange(newValue);
    };

    const incrementDate = () => {
        const currentDate = new Date(currentValue);
        currentDate.setDate(currentDate.getDate() + 1);
        updateValue(currentDate.toISOString().split('T')[0]);
    };

    const decrementDate = () => {
        const currentDate = new Date(currentValue);
        currentDate.setDate(currentDate.getDate() - 1);
        updateValue(currentDate.toISOString().split('T')[0]);
    };

    return (
        <div className='flex flex-row gap-1'>
            <button tabIndex={1} className="flex justify-center items-center flex-row px-4 bg-[#211f1c] border border-white/5 rounded" style={{ width: 20 }} type='button' onClick={decrementDate}>-</button>
            <input
                tabIndex={0}
                type="date"
                name="date"
                value={currentValue}
                onChange={(e) => updateValue(e.target.value)}
                required
                className="w-full px-3 py-2 bg-[#0f0e0c] border border-white/10 rounded text-white placeholder-gray-600 focus:outline-none focus:border-blue-500 transition"
            />
            <button tabIndex={1} className="flex justify-center items-center flex-row px-4 bg-[#211f1c] border border-white/5 rounded" style={{ width: 20 }} type='button' onClick={incrementDate}>+</button>
        </div>
    );
}