const MenuItemInformation = ({ onClick }: { onClick: () => void }) => {
    return (
        <div className="w-[445px] h-[445px] bg-modal-gradient rounded-2xl relative">
            <button
                onClick={onClick}
                className="absolute top-5 right-5 text-xl"
            >
                X
            </button>
            <div className="bg-white mt-[0.4rem] rounded-2xl h-full shadow-xl p-8 flex flex-col">
                <h2 className="text-xl font-bold pb-3">Skladniki i Alergeny</h2>

                <ul className="w-full h-full shadow-inner-lg p-3">
                    <li className="text-[rgba(0,0,0,0.5)]">skladnik 1</li>
                    <li className="text-[rgba(0,0,0,0.5)]">skladnik 2</li>
                    <li className="text-[rgba(0,0,0,0.5)]">skladnik 3</li>
                </ul>
            </div>
        </div>
    );
};

export default MenuItemInformation;
