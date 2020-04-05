export type AuthToken = string;

export type UserId = string;

export type Password = string;

export type Email = string;

export type Name = string;

export type Birthday = Date;

export type Address = string;

export type Occupation = string;

export type Employer = string;

export class User {
    public email: Email;
    public password: Password;
    public personalInfo: PersonalInfo;
}

export class PersonalInfo {
    public name: Name;
    public birthday: Birthday;
    public address: Address;
    public occupation: Occupation;
    public employer: Employer;
}

export class SessionInfo {
    public userId: UserId;
    public email: Email;
    public personalInfo: PersonalInfo;
}
