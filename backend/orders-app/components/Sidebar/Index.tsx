import { BiFoodMenu } from "react-icons/bi";
import { IoMdCloseCircleOutline } from "react-icons/io";
import { LuSettings } from "react-icons/lu";
import {
  MdOutlineBorderColor,
  MdOutlineEditCalendar,
  MdOutlineTableBar,
} from "react-icons/md";
import Icon from "../shared/Icon/Index";

const Sidebar = () => (
  <nav className="w-1/7 p-5 flex flex-col bg-gradient-to-b from-[#1E2A38] to-[#2B4B5C] text-white">
    <ul className="space-y-5 relative text-center h-full flex flex-col flex-grow">
      <li className="flex flex-col items-center hover:text-blue-300 cursor-pointer">
        <Icon el={MdOutlineTableBar} />
        <span className="text-lg">Stoliki</span>
      </li>
      <li className="flex flex-col items-center hover:text-blue-300 cursor-pointer">
        <Icon el={MdOutlineBorderColor} />
        <span className="text-lg">Zamówienia</span>
      </li>
      <li className="flex flex-col items-center hover:text-blue-300 cursor-pointer">
        <Icon el={BiFoodMenu} />
        <span className="text-lg">Karta</span>
      </li>
      <li className="flex flex-col items-center hover:text-blue-300 cursor-pointer">
        <Icon el={MdOutlineEditCalendar} />
        <span className="text-lg">Rezerwacje</span>
      </li>
      <li className="flex flex-col items-center hover:text-blue-300 cursor-pointer">
        <Icon el={IoMdCloseCircleOutline} />
        <span className="text-lg">Koniec Dnia</span>
      </li>
      <li className="flex flex-col absolute items-center bottom-0 hover:text-blue-300 cursor-pointer">
        <Icon el={LuSettings} />
        <span className="text-lg">Ustawienia</span>
      </li>
    </ul>
  </nav>
);

export default Sidebar;
