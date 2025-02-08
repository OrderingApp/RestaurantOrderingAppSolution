import { IconType } from "react-icons";

const Icon = ({ el }: { el: IconType }) => {
  const IconEl = el;

  return <IconEl className="w-10 h-10" />;
};

export default Icon;
