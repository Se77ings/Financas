import { PointerEvent, useRef } from "react";

/** Arrasto horizontal com o mouse; no toque, a rolagem nativa já faz isso. */
export function useDragScroll<T extends HTMLElement>() {
    const ref = useRef<T>(null);
    const start = useRef<{ x: number; scrollLeft: number } | null>(null);

    const stop = () => { start.current = null; };

    return {
        ref,
        onPointerDown: (event: PointerEvent<T>) => {
            if (event.pointerType !== 'mouse' || !ref.current) return;
            start.current = { x: event.clientX, scrollLeft: ref.current.scrollLeft };
        },
        onPointerMove: (event: PointerEvent<T>) => {
            if (!start.current || !ref.current) return;
            ref.current.scrollLeft = start.current.scrollLeft - (event.clientX - start.current.x);
        },
        onPointerUp: stop,
        onPointerLeave: stop,
    };
}
