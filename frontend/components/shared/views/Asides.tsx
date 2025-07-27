import clsx from 'clsx';
import { type ReactNode } from 'react';

import BottomAside, { type BottomAsideProps } from '../asides/Bottom';
import DetailsAside, { type DetailsAsideProps } from '../asides/Details';

interface AsidesViewProps {
    details: DetailsAsideProps;
    bottom: BottomAsideProps;
    children: ReactNode;
    isBottomAsideShown?: boolean;
}
const AsidesView = ({
    isBottomAsideShown,
    details,
    bottom,
}: AsidesViewProps) => (
    <div className="grid grid-cols-[1fr_224px] grid-rows-[1fr_minmax(126px,auto)] h-full">
        {/* Divs to make the desired box-shadow work */}
        {isBottomAsideShown && (
            <>
                <div className="col-start-2 row-end-2 shadow-sm-left z-10"></div>
                {/* Without this one 👇 box-shadow overflows to the 'details' aside, rather than being only visible from the left */}
                <div className="col-start-2 row-start-1 row-end-3 bg-white z-20"></div>
            </>
        )}
        <DetailsAside
            {...details}
            className={clsx(
                'col-start-2 row-span-full z-30',
                isBottomAsideShown && '!shadow-none'
            )}
        />

        <div className={clsx(!isBottomAsideShown && 'row-span-full')}></div>

        {isBottomAsideShown && <BottomAside {...bottom} />}
    </div>
);

export default AsidesView;
