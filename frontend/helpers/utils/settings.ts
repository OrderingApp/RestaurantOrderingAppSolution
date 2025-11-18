import { ForwardRefExoticComponent, RefAttributes } from 'react';
import {
    Archive,
    CircleUser,
    FileText,
    LucideProps,
    Printer,
    Settings,
} from 'lucide-react';
import languagePacks, { LanguageTypes } from '../constants/languagePacks';

interface SettingsDef {
    readonly label: string;
    readonly icon: ForwardRefExoticComponent<
        Omit<LucideProps, 'ref'> & RefAttributes<SVGSVGElement>
    >;
    readonly href: string;
}

const getSettingsItems = (language: LanguageTypes) => {
    const {
        settingsPage: {
            settings: { printerSettings, editMenu, systemInfo, archive, users },
        },
    } = languagePacks[language];

    const settingsItems: SettingsDef[] = [
        {
            label: printerSettings,
            icon: Printer,
            href: 'printer',
        },
        {
            label: editMenu,
            icon: FileText,
            href: 'menu',
        },
        {
            label: systemInfo,
            icon: Settings,
            href: 'system-information',
        },
        {
            label: archive,
            icon: Archive,
            href: 'archive',
        },
        {
            label: users,
            icon: CircleUser,
            href: 'users',
        },
    ] as const;

    return settingsItems;
};

export default getSettingsItems;
