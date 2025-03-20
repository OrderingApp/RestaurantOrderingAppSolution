const ToggleSwitch = () => {
    return (
        <div className="flex bg-white rounded-full w-48 h-11 shadow-lg items-center">
            <div className="flex-1 bg-[#2C5364] text-white flex items-center text-center justify-center rounded-full font-medium h-full">
                <span className="text-center">Odbiór</span>
            </div>
            <div className="flex-1 text-black flex items-center justify-center font-medium h-full">
                Dowóz
            </div>
        </div>
    );
};

export default ToggleSwitch;
