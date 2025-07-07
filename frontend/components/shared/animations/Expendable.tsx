import { motion, AnimatePresence } from 'framer-motion';

interface ExpandableSectionProps {
    children: React.ReactNode;
    isOpen?: boolean;
    duration?: number;
    className?: string;
}

const ExpandableSection = ({
    isOpen,
    children,
    className = '',
    duration = 0.4,
}: ExpandableSectionProps) => {
    return (
        <AnimatePresence initial={false}>
            {isOpen && (
                <motion.div
                    className={`overflow-hidden ${className}`}
                    initial={{ height: 0, opacity: 0 }}
                    animate={{ height: 'max-content', opacity: 1 }}
                    exit={{ height: 0, opacity: 0 }}
                    transition={{ duration, ease: 'easeInOut' }}
                >
                    {children}
                </motion.div>
            )}
        </AnimatePresence>
    );
};

export default ExpandableSection;
