const Categories = () => (
    <aside className="w-64 bg-gradient-to-b from-[#1E2A38] to-[#2B4B5C] text-white p-5 h-full">
    <h3 className="text-lg font-bold mb-4">Kategorie</h3>
        <ul className="space-y-4">
            <li className="hover:text-blue-300 cursor-pointer">
                Przystawki
            </li>
            <li className="hover:text-blue-300 cursor-pointer">
                Pizza
            </li>
            <li className="hover:text-blue-300 cursor-pointer">
                Calzone
            </li>
            <li className="hover:text-blue-300 cursor-pointer">
                Napoje
            </li>
        </ul>
    </aside>
);

export default Categories;