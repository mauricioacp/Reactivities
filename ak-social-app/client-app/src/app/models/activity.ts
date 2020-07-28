export interface IActivity {
    id:string;
    title:string;
    description:string;
    category:string;
    date:Date;
    city:string;
    venue:string;
}

export interface IActiviyFormValues extends Partial<IActivity>{

    time?:Date
}

export class ActivityFormValues implements IActiviyFormValues{
    id?: string=undefined;
    title: string ='';
    category: string ='';
    description: string ='';
    date?: Date=undefined;
    time?:Date= undefined;
    city:string ='';
    venue:string ='';

    constructor(init?:IActiviyFormValues){
        if(init && init.date){
            init.time=init.date
        }
        Object.assign(this,init);
    }
}