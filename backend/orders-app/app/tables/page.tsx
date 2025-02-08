import BarView from "./BarView/Index";

import { useState } from "react";

export default function Index() {
  const [currentView, setCurrentView] = useState("bar");
  return (
    <div className="flex-1 flex flex-col bg-white rounded-r-lg">
      <div className="p-5">
        <h1 className="text-xl font-bold">Wybierz stolik</h1>
        <div className="flex space-x-4 mt-4">
          <button
            className={`px-4 py-2 rounded-md ${
              currentView === "bar"
                ? "bg-[#1E2A38] text-white"
                : "bg-gray-200 text-gray-800"
            }`}
            onClick={() => setCurrentView("bar")}
          >
            Bar
          </button>
          <button className="px-4 py-2 bg-[#1E2A38] text-white rounded-md hover:bg-[#2B4B5C]">
            Kominek
          </button>
          <button className="px-4 py-2 bg-[#1E2A38] text-white rounded-md hover:bg-[#2B4B5C]">
            Bilardownia
          </button>
          <button className="px-4 py-2 bg-[#1E2A38] text-white rounded-md hover:bg-[#2B4B5C]">
            Góra
          </button>
          <button className="px-4 py-2 bg-[#1E2A38] text-white rounded-md hover:bg-[#2B4B5C]">
            Ogródek
          </button>
        </div>
      </div>
      <div className="flex-1">{currentView === "bar" && <BarView />}</div>
      <div className="flex justify-end space-x-4 p-5">
        <button className="px-6 py-2 bg-red-500 text-white rounded-md hover:bg-red-600">
          Usuń
        </button>
        <button className="px-6 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600">
          Dodaj
        </button>
      </div>
    </div>
  );
}
