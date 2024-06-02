// TypeScript representation of the DimensionVm C# class
export interface Rooms {
    height: number;
    width: number;
}

// TypeScript representation of the FormatVm C# class
export interface FormatVm {
    id: number;
    key: string;
    description?: string;
}

// TypeScript representation of the LinkVm C# class
export interface LinkVm {
    href: string;
    method: string;
}

// TypeScript representation of the VmWrapper C# generic class
export interface VmWrapper<T> {
    vm?: T;
    message?: string;
    _links: { [key: string]: LinkVm };
}

export interface Url{
    href?: string;
    method?: string;
}

export interface TaggedVm{
    imageId: number;
    classification: string;
}