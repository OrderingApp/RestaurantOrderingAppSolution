import Image from 'next/image';

import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs';
import Button from '@/components/shared/button/Button';

import EditIcon from '@/public/images/svg/edit.svg';

interface TablesHeaderProps {
    onTabChange: (newTab: (typeof tabsMock)[number]['value']) => void;
}

const TablesHeader = ({ onTabChange }: TablesHeaderProps) => (
    <header className="flex justify-center items-center gap-9">
        <Tabs defaultValue={tabsMock[0].value} onValueChange={onTabChange}>
            <TabsList className="p-0 h-auto bg-white rounded-3xl text-black text-sm">
                {tabsMock.map(({ label, value }) => (
                    <TabsTrigger
                        key={value}
                        value={value}
                        className="py-4 px-10 font-semibold rounded-3xl data-[state=active]:bg-quaternary data-[state=active]:text-white"
                    >
                        {label}
                    </TabsTrigger>
                ))}
            </TabsList>
        </Tabs>

        <Button
            variant="quaternary"
            size="xxs"
            className="rounded-xl aspect-square h-[46px]"
        >
            <Image src={EditIcon} alt="Edit tables" />
        </Button>
    </header>
);

const tabsMock = [
    { label: 'Bar', value: 'bar' },
    { label: 'Kominek', value: 'kominek' },
    { label: 'Bilardownia', value: 'bilardownia' },
    { label: 'Góra', value: 'góra' },
];

export default TablesHeader;
