import { useState } from "react";

export default function Content({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {
    const [selected, setSelected] = useState(false);

    return (
        <div className={`flex flex-col bg-blue-500 px-3 justify-center border-r cursor-pointer ${selected ? 'bg-blue-600' : ''}`} onClick={() => handleClick()}>
            <p className="text-center">{children}</p>
        </div>
    )

    function handleClick() {
        setSelected(!selected)
    }

}
