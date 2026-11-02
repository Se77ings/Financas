import { useState } from 'react';
import { SaveMode } from './SaveButton';

// Escopo de módulo: sobrevive ao desmonte do formulário, mas zera ao recarregar a página.
const lastModeByForm = new Map<string, SaveMode>();

export function useSaveMode(formKey: string): [SaveMode, (mode: SaveMode) => void] {
    const [saveMode, setSaveModeState] = useState<SaveMode>(() => lastModeByForm.get(formKey) ?? 'close');

    function setSaveMode(mode: SaveMode) {
        lastModeByForm.set(formKey, mode);
        setSaveModeState(mode);
    }

    return [saveMode, setSaveMode];
}
