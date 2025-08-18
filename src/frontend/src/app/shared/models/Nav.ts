
export interface Nav {
    section: string;
    links: NavLink[];
}

export interface NavLink {
    link: string;
    icon: string;
    title: string;
}