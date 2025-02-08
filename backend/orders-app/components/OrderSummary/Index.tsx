const OrderSummary = () => (
    <div className="w-1/3 bg-gray-100 p-5 flex flex-col h-full">
        <h2 className="text-lg font-bold mb-4">Stolik 4</h2>
        <ul className="space-y-2 flex-1">
            <li className="flex justify-between">
                <span>2x Pizza 1 (33cm)</span>
                <span>zł 76.00</span>
            </li>
            <li className="flex justify-between">
                <span>2x Pizza 1 (33cm)</span>
                <span>zł 66.00</span>
            </li>
        </ul>
        <div className="border-t pt-4 mt-4 flex items-center justify-between">
            <div className="flex space-x-2">
                <button className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">
                    Zapisz
                </button>
                <button className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600">
                    Zapłać
                </button>
            </div>
            <span className="font-bold">zł 400.00</span>
        </div>
    </div>
);

export default OrderSummary;