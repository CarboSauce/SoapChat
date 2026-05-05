import { gql } from '@apollo/client'

export const LOGIN = gql`
  mutation Login($username: String!, $password: String!) {
    login(username: $username, password: $password) {
      user {
        id
        name
      }
      token
      message
    }
  }
`

export const REGISTER = gql`
  mutation Register($username: String!, $password: String!) {
    register(username: $username, password: $password) {
      user {
        id
        name
      }
      token
      message
    }
  }
`

export const GET_ME = gql`
  query GetMe {
    me {
      id
      username
    }
  }
`

export const GET_USER_GROUPS = gql`
  query GetUserGroups($userId: String!) {
    searchGroupsByMember(userId: $userId) {
      id
      name
      members {
        id
        name
      }
      createdAt
    }
  }
`

export const GET_GROUP = gql`
  query GetGroup($id: String!) {
    group(id: $id) {
      id
      name
      members {
        id
        name
      }
      createdAt
    }
  }
`

export const CREATE_GROUP = gql`
  mutation CreateGroup($name: String!) {
    createGroup(name: $name) {
      id
      name
      members {
        id
        name
      }
      createdAt
    }
  }
`

export const ADD_GROUP_MEMBER = gql`
  mutation AddMember($groupId: String!, $userId: String!) {
    addMember(groupId: $groupId, userId: $userId) {
      id
      name
      members {
        id
        name
      }
      createdAt
    }
  }
`

export const REMOVE_GROUP_MEMBER = gql`
  mutation RemoveMember($groupId: String!, $userId: String!) {
    removeMember(groupId: $groupId, userId: $userId) {
      id
      name
      members {
        id
        name
      }
      createdAt
    }
  }
`

export const GET_MESSAGES = gql`
  query GetMessages($groupId: String!, $skip: Int!, $limit: Int!) {
    messages(groupId: $groupId, skip: $skip, limit: $limit) {
      id
      text
      sender {
        id
        name
      }
      sentAt
    }
  }
`

export const GET_NEWER_MESSAGES = gql`
  query GetNewerMessages($groupId: String!, $lastMessageId: String!) {
    newerMessages(groupId: $groupId, lastMessageId: $lastMessageId) {
      id
      text
      sender {
        id
        name
      }
      sentAt
    }
  }
`

export const CREATE_MESSAGE = gql`
  mutation CreateMessage($groupId: String!, $text: String!) {
    createMessage(groupId: $groupId, text: $text) {
      id
      text
      sender {
        id
        name
      }
      sentAt
    }
  }
`

export const SEARCH_USERS = gql`
  query SearchUsers($nameSearch: String!) {
    searchUsers(nameSearch: $nameSearch) {
      id
      username
    }
  }
`

export const GET_USER = gql`
  query GetUser($id: String!) {
    user(id: $id) {
      id
      username
    }
  }
`
