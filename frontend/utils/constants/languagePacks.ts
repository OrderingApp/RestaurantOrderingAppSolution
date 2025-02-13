import plPack from '@/utils/constants/languagePacks/polish';

export const languagePacks = {
    PL: plPack,
};

export interface languagePack {
    loginPage: {
        appName: string;
        login: string;
        password: string;
        enter: string;
    };
    menuBar: {
        tables: {
            name: string;
        };
        orders: string;
        menu: string;
        reservations: string;
        endDay: string;
        settings: string;
    };
}
