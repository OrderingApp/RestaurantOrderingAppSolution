const MenuGrid = () => {
    const items = [
        { name: 'Pizza 1', price: '33 zł', size: '33cm' },
        { name: 'Pizza 2', price: '45 zł', size: '45cm' },
    ];

    return (
        <div className="flex-1 grid grid-cols-3 gap-4 items-start bg-white p-5 h-full">
            {items.map((item, index) => (
                <div key={index} className="border rounded-lg p-4 shadow hover:shadow-lg">
                    <h3 className="text-lg font-bold">{item.name}</h3>
                    <p>{item.size} - {item.price}</p>
                    <button>Dodaj</button>
                </div>
            ))}
        </div>
    );
};

export default MenuGrid;