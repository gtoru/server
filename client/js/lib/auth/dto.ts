import { User, PersonalInfo, SessionInfo } from "./models";

export class UserDto {
    public email: string;
    public password: string;
    public name: string;
    public birthday: string;
    public address: string;
    public occupation: string;
    public employer: string;

    public static fromModel(userModel: User): UserDto {
        return {
            email: userModel.email,
            password: userModel.password,
            address: userModel.personalInfo?.address,
            birthday: userModel.personalInfo?.birthday.toISOString(),
            employer: userModel.personalInfo?.employer,
            name: userModel.personalInfo?.name,
            occupation: userModel.personalInfo?.occupation,
        };
    }
}

export class PersonalInfoDto {
    public name: string;
    public birthday: string;
    public address: string;
    public occupation: string;
    public employer: string;

    public static fromModel(personalInfo: PersonalInfo): PersonalInfoDto {
        return {
            address: personalInfo.address,
            birthday: personalInfo.birthday.toISOString(),
            employer: personalInfo.employer,
            name: personalInfo.name,
            occupation: personalInfo.occupation,
        };
    }

    public static toModel(personalInfo: PersonalInfoDto): PersonalInfo {
        return {
            address: personalInfo.address,
            birthday: new Date(personalInfo.birthday),
            employer: personalInfo.employer,
            name: personalInfo.name,
            occupation: personalInfo.occupation,
        };
    }
}

export class SessionInfoDto {
    public userId: string;
    public email: string;
    public personalInfo: PersonalInfoDto;

    public static fromModel(sessionInfo: SessionInfo): SessionInfoDto {
        return {
            email: sessionInfo.email,
            userId: sessionInfo.userId,
            personalInfo: PersonalInfoDto.fromModel(sessionInfo.personalInfo),
        };
    }

    public static toModel(sessionInfo: SessionInfoDto): SessionInfo {
        return {
            email: sessionInfo.email,
            userId: sessionInfo.userId,
            personalInfo: PersonalInfoDto.toModel(sessionInfo.personalInfo),
        };
    }
}
