'use client';

import Image from 'next/image';

import Modal from './Modal';
import Button from '@/components/shared/button/Button';

import { useLanguage } from '@/providers/LanguageProvider';

import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';

interface AlertDialogProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => void;
}

const AlertDialog = ({ isOpen, onClose, onConfirm }: AlertDialogProps) => {
    const { language } = useLanguage();

    const {
        alertDialog: { attentionTitle, attentionContent, closeBtn, confirmBtn },
    } = languagePacks[language];

    return (
        <Modal isOpen={isOpen} onClose={onClose} zIndex={60}>
            <div className="w-[445px] h-[445px] bg-attention-gradient rounded-2xl relative shadow-2xl pt-[0.4rem]">
                <button
                    onClick={onClose}
                    className="absolute top-5 right-5 text-xl z-10 hover:opacity-70 transition-opacity text-gray-400 hover:text-black"
                >
                    X
                </button>
                <div className="bg-white h-full rounded-2xl p-6 pb-8 flex flex-col items-center justify-center gap-6">
                    <Image
                        src={ICONS.ALERT}
                        alt="alert"
                        width={136}
                        height={136}
                    />
                    <div className="flex flex-col justify-center items-center pt-2">
                        <h2 className="text-xl font-bold text-center text-black">
                            {attentionTitle}
                        </h2>
                    </div>

                    <div className="w-full px-4">
                        <p className="text-gray-600 text-sm text-center">
                            {attentionContent}
                        </p>
                    </div>

                    <div className="flex justify-center gap-4 mt-2 px-4 w-full">
                        <Button
                            variant="outline"
                            onClick={onClose}
                            className="border-gray-400 hover:border-gray-600 text-gray-600 hover:text-gray-800 w-full"
                        >
                            {closeBtn}
                        </Button>
                        <Button
                            variant="danger"
                            onClick={onConfirm}
                            className="w-full"
                        >
                            {confirmBtn}
                        </Button>
                    </div>
                </div>
            </div>
        </Modal>
    );
};

export default AlertDialog;
