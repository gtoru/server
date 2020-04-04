export type AuthToken = string;

export type Password = string;

export type Email = string;

export type Name = string;

export type Birthday = Date;

export type Address = string;

export type Occupation = string;

export type Employer = string;

export class User {
    public name: Name;
    public email: Email;
    public password: Password;
    public birthday: Birthday;
    public address: Address;
    public occupation: Occupation;
    public employer: Employer;
}
