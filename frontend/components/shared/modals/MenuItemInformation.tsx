'use client';

import Image from 'next/image';

import {
    Table,
    TableHead,
    TableHeader,
    TableRow,
    TableBody,
    TableCell,
} from '@/components/ui/table';

import { useLanguage } from '@/providers/LanguageProvider';

import { MODAL_HEIGHT, MODAL_WIDTH } from '@/helpers/constants/constants';
import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';

const DUMMY_DATA = [
    {
        ingredient: 'Sos pomidorwy',
        allergens: 'gorczyca, gluten, mleko, jajka, sezam, soja i seler',
    },
    {
        ingredient: 'Mozzarella',
        allergens: 'gorczyca, gluten, mleko, jajka, sezam, soja i seler',
    },
    {
        ingredient: 'Kurczak',
        allergens: 'gorczyca, gluten, mleko, jajka, sezam, soja i seler',
    },
    {
        ingredient: 'Salami',
        allergens: 'gorczyca, gluten, mleko, jajka, sezam, soja i seler',
    },
];

const MenuItemInformation = ({ onClick }: { onClick: () => void }) => {
    const { language } = useLanguage();

    const {
        menuItemInformationModal: { title, ingredientsTitle, allergensTitle },
    } = languagePacks[language];

    return (
        <div
            className="bg-modal-gradient rounded-2xl relative"
            style={{ width: MODAL_WIDTH, height: MODAL_HEIGHT }}
        >
            <button
                onClick={onClick}
                className="absolute top-5 right-5 text-xl"
            >
                <Image src={ICONS.CLOSE} alt="close" />
            </button>
            <div className="bg-white mt-[0.4rem] rounded-2xl h-full shadow-xl p-8 px-4 flex flex-col ">
                <h2 className="text-xl font-bold pb-3">{title}</h2>

                <Table className="p-3 [&_tr]:border-none [&_tr:nth-child(odd)]:bg-[#EBE9E9] [&_tr:nth-child(even)]:bg-[#F6F2F2] !shadow-inner-lg">
                    <TableHeader>
                        <TableRow className="!bg-white">
                            <TableHead className="w-[120px] text-center text-base font-bold">
                                {ingredientsTitle}
                            </TableHead>
                            <TableHead className="text-center text-base font-bold">
                                {allergensTitle}
                            </TableHead>
                        </TableRow>
                    </TableHeader>

                    <TableBody className="w-full h-full">
                        {DUMMY_DATA.map((item) => (
                            <TableRow key={item.ingredient}>
                                <TableCell className="font-bold border-r-[1px] border-zinc-700 p-3 px-2 text-xs">
                                    {item.ingredient}
                                </TableCell>
                                <TableCell className="text-xs ">
                                    {item.allergens}
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>
        </div>
    );
};

export default MenuItemInformation;

//TODO Add real data and change improve ui styling
