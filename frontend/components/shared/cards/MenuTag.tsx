import clsx from 'clsx';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';

interface MenuTagProps {
    id: string;
    name: string;
}

const MenuTag = ({ id, name }: MenuTagProps) => {
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    const tags = searchParams.getAll('tag');

    const isActive = tags.includes(id);

    const toggleMenuTag = () => {
        const newParams = new URLSearchParams(searchParams.toString());
        const newTags = isActive
            ? tags.filter((tag) => tag !== id)
            : [...tags, id];

        newParams.delete('tag');
        newTags.forEach((tag) => newParams.append('tag', tag));

        router.push(`${pathname}?${newParams.toString()}`);
    };

    return (
        <li
            className={clsx(
                'w-28 h-9 rounded-lg shadow-[0px_4px_4px_0px_#00000040] transition-all',
                isActive ? 'bg-primary text-white' : 'bg-white text-black'
            )}
            key={id}
        >
            <button
                onClick={toggleMenuTag}
                className="w-full h-full py-2 px-4 text-center flex items-center justify-center"
            >
                {name}
            </button>
        </li>
    );
};

export default MenuTag;
