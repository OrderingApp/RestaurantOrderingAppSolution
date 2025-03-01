import type { ReactElement, ReactNode } from 'react';

type LiElement = ReactElement<{ children: ReactNode }, 'li'>;

export type LiElements = LiElement | LiElement[];
