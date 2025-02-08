const KominekView = () => {
  return (
    <div className="relative flex-1 bg-gray-100 rounded-lg">
      <div className="absolute top-10 left-10 w-[150px] h-[100px] flex flex-col items-center justify-center bg-blue-500 text-white rounded-lg p-4 shadow">
        <p className="text-lg font-bold">K1</p>
        <p>REZERWACJA</p>
        <p>14:30</p>
      </div>
      <div className="absolute top-10 right-10 w-[150px] h-[100px] flex flex-col items-center justify-center bg-[#1E2A38] text-white rounded-lg p-4 shadow">
        <p className="text-lg font-bold">P1</p>
        <p className="text-red-500">zł 400.00</p>
      </div>
      <div className="absolute bottom-10 left-10 w-[150px] h-[100px] flex flex-col items-center justify-center bg-gray-300 text-gray-800 rounded-lg p-4 shadow">
        <p className="text-lg font-bold">L2</p>
      </div>
      <div className="absolute bottom-10 right-10 w-[150px] h-[100px] flex flex-col items-center justify-center bg-gray-300 text-gray-800 rounded-lg p-4 shadow">
        <p className="text-lg font-bold">P2</p>
      </div>
    </div>
  );
};

export default KominekView;
