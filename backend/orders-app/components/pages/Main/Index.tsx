import Categories from "@/components/Categories/Index";
import MenuGrid from "@/components/MenuGrid/Index";
import OrderSummary from "@/components/OrderSummary/Index";

const MainPage = () => (
    <div className="flex flex-1">
        <OrderSummary/>
        <MenuGrid/>
        <Categories/>
    </div>
);

export default MainPage;