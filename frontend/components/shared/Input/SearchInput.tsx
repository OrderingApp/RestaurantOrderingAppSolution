'use client';
import Image from 'next/image';
import Input from './Input';
import searchSvg from '@/public/images/svg/search.svg';
import { useRouter, useSearchParams } from 'next/navigation';

interface SearchInputProps {
    placeholder?: string;
    className?: string;
}

const SearchInput = ({ placeholder, className }: SearchInputProps) => {
    const router = useRouter();
    const searchParams = useSearchParams();

    const changeInputValue = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newParams = new URLSearchParams(searchParams.toString());
        newParams.set('name', e.target.value);
        router.push(`?${newParams.toString()}`);
    };

    return (
        <Input
            type="search"
            placeholder={placeholder}
            inputClassName={`[&::placeholder]:text-black bg-white shadow-[0px_4px_4px_0px_#00000040] pl-8 ml-4 ${className}`}
            icon={<Image src={searchSvg} alt="searchIcon" />}
            iconClassName="left-7 top-[11px] w-4"
            onChange={changeInputValue}
        />
    );
};

export default SearchInput;
