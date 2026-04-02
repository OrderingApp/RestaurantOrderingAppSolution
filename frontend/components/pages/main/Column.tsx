const Column = ({
    title,
    count,
    children,
}: {
    title: string;
    count: number;
    children: React.ReactNode;
}) => (
    <section className="flex flex-col flex-1 min-w-0">
        <h2 className="font-bold text-md mb-3">
            {title}{' '}
            <span className="text-dark-gray font-normal">({count})</span>
        </h2>
        <div className="flex flex-col gap-2 overflow-y-auto hide-scrollbar pr-1">
            {children}
        </div>
    </section>
);

export default Column;
