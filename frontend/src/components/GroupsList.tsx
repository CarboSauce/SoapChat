import { useState } from 'react'
import { useQuery, useMutation } from '@apollo/client'
import { GET_USER_GROUPS, CREATE_GROUP, SEARCH_USERS, ADD_GROUP_MEMBER } from '../graphql'
import { useAuth } from '../context/AuthContext'
import '../styles/groups.css'

interface GroupsListProps {
  selectedGroupId: string | null
  onSelectGroup: (groupId: string) => void
}

export default function GroupsList({ selectedGroupId, onSelectGroup }: GroupsListProps) {
  const { user } = useAuth()
  const [showCreateModal, setShowCreateModal] = useState(false)
  const [showAddMemberModal, setShowAddMemberModal] = useState(false)
  const [newGroupName, setNewGroupName] = useState('')
  const [memberSearch, setMemberSearch] = useState('')
  const [selectedGroupForMember, setSelectedGroupForMember] = useState<string | null>(null)
  const [error, setError] = useState('')

  const { data, loading, error: queryError, refetch } = useQuery(GET_USER_GROUPS, {
    variables: { userId: user?.id },
    skip: !user?.id,
    pollInterval: 5000
  })

  const [createGroup] = useMutation(CREATE_GROUP, {
    onCompleted: () => {
      setNewGroupName('')
      setShowCreateModal(false)
      refetch()
    },
    onError: (err) => setError(err.message)
  })

  const { data: searchData } = useQuery(SEARCH_USERS, {
    variables: { nameSearch: memberSearch },
    skip: !memberSearch || memberSearch.length < 2
  })

  const [addMember] = useMutation(ADD_GROUP_MEMBER, {
    onCompleted: () => {
      setMemberSearch('')
      setShowAddMemberModal(false)
      refetch()
    },
    onError: (err) => setError(err.message)
  })

  const handleCreateGroup = (e: React.FormEvent) => {
    e.preventDefault()
    setError('')
    if (!newGroupName.trim()) {
      setError('Group name is required')
      return
    }
    createGroup({ variables: { name: newGroupName } })
  }

  const handleAddMember = (userId: string) => {
    setError('')
    if (!selectedGroupForMember) {
      setError('No group selected')
      return
    }
    addMember({
      variables: {
        groupId: selectedGroupForMember,
        userId: userId
      }
    })
  }

  const groups = data?.searchGroupsByMember || []

  return (
    <div className="groups-list">
      <div className="groups-header">
        <h2>Groups</h2>
        <button
          onClick={() => setShowCreateModal(true)}
          className="btn btn-small"
        >
          + New
        </button>
      </div>

      {queryError && (
        <div className="error-message">{queryError.message}</div>
      )}

      {loading && <div className="loading">Loading groups...</div>}

      <div className="groups-items">
        {groups.map((group: any) => (
          <div
            key={group.id}
            className={`group-item ${selectedGroupId === group.id ? 'active' : ''}`}
            onClick={() => onSelectGroup(group.id)}
          >
            <div className="group-name">{group.name}</div>
            <div className="group-members">{group.members.length} members</div>
          </div>
        ))}
      </div>

      {showCreateModal && (
        <div className="modal">
          <div className="modal-content">
            <h3>Create New Group</h3>
            <form onSubmit={handleCreateGroup}>
              <div className="form-group">
                <label htmlFor="group-name">Group Name:</label>
                <input
                  id="group-name"
                  type="text"
                  value={newGroupName}
                  onChange={(e) => setNewGroupName(e.target.value)}
                  placeholder="Enter group name"
                  className="form-input"
                  autoFocus
                />
              </div>
              {error && <div className="error-message">{error}</div>}
              <div className="modal-buttons">
                <button type="submit" className="btn btn-primary">
                  Create
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowCreateModal(false)
                    setError('')
                  }}
                  className="btn btn-secondary"
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showAddMemberModal && (
        <div className="modal">
          <div className="modal-content">
            <h3>Add Member to {groups.find((g: any) => g.id === selectedGroupForMember)?.name}</h3>
            <input
              type="text"
              value={memberSearch}
              onChange={(e) => setMemberSearch(e.target.value)}
              placeholder="Search users..."
              className="form-input"
              autoFocus
            />
            {searchData?.searchUsers && (
              <div className="search-results">
                {searchData.searchUsers.map((u: any) => (
                  <div key={u.id} className="search-result">
                    <span>{u.username}</span>
                    <button
                      onClick={() => handleAddMember(u.id)}
                      className="btn btn-small"
                    >
                      Add
                    </button>
                  </div>
                ))}
              </div>
            )}
            {error && <div className="error-message">{error}</div>}
            <div className="modal-buttons">
              <button
                onClick={() => {
                  setShowAddMemberModal(false)
                  setMemberSearch('')
                  setError('')
                }}
                className="btn btn-secondary"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}

      {selectedGroupId && (
        <button
          onClick={() => {
            setShowAddMemberModal(true)
            setSelectedGroupForMember(selectedGroupId)
          }}
          className="btn btn-secondary"
          style={{ marginTop: '10px', width: '100%' }}
        >
          Add Member
        </button>
      )}
    </div>
  )
}
